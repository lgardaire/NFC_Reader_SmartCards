package fr.unice.polytech.smartcards.hcexplorer;

import android.nfc.cardemulation.HostApduService;
import android.os.Bundle;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.charset.Charset;
import java.util.Arrays;

/**
 * Created by user on 07/02/2019.
 */

public class cCardService extends HostApduService {

    private static final String NDEF_FILE_NAME = "ndef_file.txt";
    private static final String CC_FILE_NAME = "cc_file.txt";
    private static final String CC_FILE_CONTENT = "12000F20003B00340406E104080000";

    private File ndefFile = null;
    private File ccFile = null;

    /**
     * E103h => CC file
     * 8101h => NDEF file
     */
    private File selectedFile;

    private int[] selectApplicationResult = null;
    private int[] selectFileResult = null;

    @Override
    public byte[] processCommandApdu(byte[] apdu, Bundle bundle) {
        int[] apduUnsigned = new int[apdu.length];
        for (byte i = 0; i < apdu.length; i++) {
            apduUnsigned[i] = apdu[i] & 0xFF;
        }
        int cla = apduUnsigned[0];
        // CLA check
        if (cla != 0x00) {
            return intArrayToByteArray(new int[]{(int) 0x6E, (int) 0x00}); // unknown CLA
        }
        int ins = apduUnsigned[1];
        // INS check
        if (ins == 0xA4) {
            // SELECT
            int p1 = apduUnsigned[2];
            if (p1 == 0x04) {
                // SELECT APPLICATION
                selectApplicationResult = selectApplication(apduUnsigned);
                return intArrayToByteArray(selectApplicationResult);
            } else if (p1 == 0x00) {
                // SELECT FILE
                if (isOK(selectApplicationResult)) {
                    selectFileResult = selectFile(apduUnsigned);
                    return intArrayToByteArray(selectFileResult);
                } else {
                    return intArrayToByteArray(new int[]{(int) 0x69, (int) 0x86}); // etat non conforme
                }
            } else {
                return intArrayToByteArray(new int[]{(int) 0x6A, (int) 0x86}); // incorrect P1/P2 SELECT
            }
        } else if (ins == 0xB0) {
            return isOK(selectFileResult) ? intArrayToByteArray(readBinary(apduUnsigned)) : intArrayToByteArray(new int[]{(int) 0x69, (int) 0x86}); // etat non conforme
        } else if (ins == 0xD6) {
            return isOK(selectFileResult) ? intArrayToByteArray(updateBinary(apduUnsigned)) : intArrayToByteArray(new int[]{(int) 0x69, (int) 0x86}); // etat non conforme
        } else {
            return intArrayToByteArray(new int[]{(int) 0x6D, (int) 0x00}); // unknown INS
        }
    }

    @Override
    public void onDeactivated(int reason) {

    }

    @Override
    public void onCreate() {
        createCCFile();
        for (File file : getFilesDir().listFiles()) {
            if (file.getName().equals(NDEF_FILE_NAME)) {
                ndefFile = file;
                break;
            }
        }
        if (ndefFile == null) {
            ndefFile = new File(getFilesDir(), NDEF_FILE_NAME);
        }
    }

    private void createCCFile() {
        ccFile = new File(getFilesDir(), CC_FILE_NAME);
        try {
            new BufferedWriter(new FileWriter(ccFile)).write(CC_FILE_CONTENT);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public int[] selectApplication(int[] apdu) {
        int cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new int[]{(int) 0x6E, (int) 0x00}; // unknown CLA
        }
        // INS check
        int ins = apdu[1];
        if (ins == 0xA4) { // SELECT
            // Getting P1 and P2
            int p1 = apdu[2];
            int p2 = apdu[3];
            if (p1 == 0x04) { // SELECT APPLICATION
                if (p2 == 0x00) {
                    int lc = apdu[4];
                    if (lc == 0x07) {
                        int[] data = Arrays.copyOfRange(apdu, 5, 5 + lc); // 5 + Lc = 12
                        int[] validData = {(int) 0xD2, (int) 0x76, (int) 0x00, (int) 0x00, (int) 0x85, (int) 0x01, (int) 0x01};
                        if (Arrays.equals(data, validData)) {
                            int le = apdu[12];
                            if (le == 0x00) {
                                return new int[]{(int) 0x90, (int) 0x00}; // OK
                            } else {
                                return new int[]{(int) 0x6C, (int) 0x00}; // incorrect Le
                            }
                        } else {
                            return new int[]{(int) 0x6A, (int) 0x82}; // unknown AID/LID
                        }
                    } else {
                        return new int[]{(int) 0x67, (int) 0x00}; // incorrect Lc
                    }
                } else {
                    return new int[]{(int) 0x6A, (int) 0x86}; // incorrect P1/P2 SELECT
                }
            } else {
                return new int[]{(int) 0x6A, (int) 0x86}; // incorrect P1/P2 SELECT
            }
        } else {
            return new int[]{(int) 0x6D, (int) 0x00}; // unknown INS
        }

    }

    public int[] selectFile(int[] apdu) {
        int cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new int[]{(int) 0x6E, (int) 0x00}; // unknown CLA
        }
        // INS check
        int ins = apdu[1];
        if (ins == 0xA4) { // SELECT
            // Getting P1 and P2
            int p1 = apdu[2];
            int p2 = apdu[3];
            if (p1 == 0x00 && p2 == 0x0C) {
                int lc = apdu[4];
                if (lc == 0x02) {
                    int[] data = Arrays.copyOfRange(apdu, 5, 5 + lc); // 5 + Lc = 7
                    int[] ccData = {(int) 0xE1, (int) 0x03};
                    int[] ndefData = {(int) 0x81, (int) 0x01};
                    if (Arrays.equals(data, ccData)) {
                        selectedFile = ccFile;
                        return new int[]{(int) 0x90, (int) 0x00}; // OK
                    } else if (Arrays.equals(data, ndefData)) {
                        selectedFile = ndefFile;
                        return new int[]{(int) 0x90, (int) 0x00}; // OK
                    } else {
                        return new int[]{(int) 0x6A, (int) 0x82}; // unknown AID/LID
                    }
                } else {
                    return new int[]{(int) 0x67, (int) 0x00}; // incorrect Lc
                }
            } else {
                return new int[]{(int) 0x6A, (int) 0x86}; // incorrect P1/P2 SELECT
            }
        } else {
            return new int[]{(int) 0x6D, (int) 0x00}; // unknown INS
        }

    }

    public int[] readBinary(int[] apdu) {
        int cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new int[]{(int) 0x6E, (int) 0x00}; // unknown CLA
        }
        int ins = apdu[1];
        // INS check
        if (ins != 0xB0) {
            return new int[]{(int) 0x6D, (int) 0x00}; // unknown INS
        }
        int[] offset = {apdu[2], apdu[3]};
        int le = apdu[4];
        // TODO: 14/02/2019 check offset + le <= maxLe, sinon return 6C00
        try {
            // FIXME: 14/02/2019 what should we do with the file content ?
            System.out.println(getFileContent(offset[0] + offset[1], le));
            return new int[]{(int) 0x90, (int) 0x00}; // OK
        } catch (IOException e) {
            e.printStackTrace();
            return new int[]{(int) 0x42, (int) 0x69}; // exception management
        }
    }

    public int[] updateBinary(int[] apdu) {
        int cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new int[]{(int) 0x6E, (int) 0x00}; // unknown CLA
        }
        int ins = apdu[1];
        // INS check
        if (ins != 0xD6) {
            return new int[]{(int) 0x6D, (int) 0x00}; // unknown INS
        }
        int[] offset = {apdu[2], apdu[3]};
        int lc = apdu[4];
        // TODO: 14/02/2019 check offset + lc <= maxLc, sinon return 6A87
        int[] contentToWrite = Arrays.copyOfRange(apdu, 5, 5 + lc);
        int[] resToPrint = new int[contentToWrite.length];
        for (int i = 0; i < contentToWrite.length; i++) {
            resToPrint[i] = (int) contentToWrite[i];
        }
        return writeToFile(resToPrint);
    }

    private int[] writeToFile(int[] contentToWrite) {
        try {
            new BufferedWriter(new FileWriter(selectedFile)).write(new String(intArrayToByteArray(contentToWrite), Charset.forName("UTF-8")));
            return new int[]{(int) 0x90, (int) 0x00}; // OK
        } catch (IOException e) {
            e.printStackTrace();
            return new int[]{(int) 0x42, (int) 0x69}; // exception management
        }
    }

    private boolean isOK(int[] returnedCode) {
        return returnedCode == new int[]{(int) 0x90, (int) 0x00};
    }

    private byte[] intArrayToByteArray(int[] byteArray) {
        byte[] res = new byte[byteArray.length];
        for (int i = 0; i < byteArray.length; i++){
            res[i] = (byte) (byteArray[i] & 0xFF);
        }
        return res;
    }

    /**
     * Returns the content from the CC or NDEF file.
     *
     * @return
     */
    private String getFileContent(int offset, int le) throws IOException {
        // TODO: 14/02/2019 check this
        return getFileAsString(selectedFile).substring(offset, offset + le); //.getBytes(Charset.forName("UTF-8"));
    }

    private String getFileAsString(File file) throws IOException {
        StringBuilder sb = new StringBuilder();
        BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(file)));
        String line;
        while ((line = br.readLine()) != null) {
            sb.append(line);
        }
        return sb.toString();
    }

}