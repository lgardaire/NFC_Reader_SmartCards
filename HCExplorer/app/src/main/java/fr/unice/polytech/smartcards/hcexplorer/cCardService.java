package fr.unice.polytech.smartcards.hcexplorer;

import android.content.Context;
import android.nfc.cardemulation.HostApduService;
import android.os.Bundle;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
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
    private static final String NDEF_FILE_CONTENT = "0022D1021D5370910111550170617261676F6E2D72666964EE636F6D5101045400504944";

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
        createNDEFFile();
//        for (File file : getFilesDir().listFiles()) {
//            if (file.getName().equals(NDEF_FILE_NAME)) {
//                ndefFile = file;
//                break;
//            }
//        }
//        if (ndefFile == null) {
//            ndefFile = new File(getFilesDir(), NDEF_FILE_NAME);
//        }
    }

    private void createCCFile() {
//        int[] ccContent = new int[]{0x00, 0x0F, 0x20, 0x00, 0x3B, 0x00, 0x34, 0x04, 0x06, 0xE1, 0x04, 0x00, 0x32, 0x00, 0x00};
        ccFile = new File(getFilesDir(), CC_FILE_NAME);
        FileOutputStream fos;
        try {
            fos = openFileOutput(ccFile.getName(), Context.MODE_PRIVATE);
            fos.write(CC_FILE_CONTENT.getBytes());
            fos.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private void createNDEFFile() {
        ndefFile = new File(getFilesDir(), NDEF_FILE_NAME);
        FileOutputStream fos;
        try {
            fos = openFileOutput(ndefFile.getName(), Context.MODE_PRIVATE);
            fos.write(NDEF_FILE_CONTENT.getBytes());
            fos.close();
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
                                return new int[]{(int) 0x03, (int) 0x90, (int) 0x00}; // OK
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
//                    int[] ndefData = {(int) 0x81, (int) 0x01};
                    System.out.println("data" + Arrays.toString(data));
                    if (Arrays.equals(data, ccData)) {
                        selectedFile = ccFile;
                        return new int[]{(int) 0x03, (int) 0x90, (int) 0x00}; // OK
//                    } else if (Arrays.equals(data, ndefData)) {
//                        selectedFile = ndefFile;
//                        return new int[]{(int) 0x03, (int) 0x90, (int) 0x00}; // OK
                    } else {
                        selectedFile = ndefFile;
                        return new int[]{(int) 0x03, (int) 0x90, (int) 0x00}; // OK
//                        return new int[]{(int) 0x6A, (int) 0x82}; // unknown AID/LID
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
            int[] maxLe = byteArrayToIntArray(getFileContent(3, 2));
            if(offset[0] + offset[1] + le > maxLe[0] + maxLe[1]){
                return new int[]{(int) 0x6C, (int) 0x00};
            }
            int[] fileContent = byteArrayToIntArray(getFileContent(offset[0] + offset[1], le));
            int[] ccLen = new int[]{fileContent.length + 1};
            int[] returnCode = new int[]{0x90, 0x00};
            int[] resArray = new int[fileContent.length + 3];
            System.arraycopy(ccLen, 0, resArray, 0, ccLen.length);
            System.arraycopy(fileContent, 0, resArray, ccLen.length, fileContent.length);
            System.arraycopy(returnCode, 0, resArray, ccLen.length + fileContent.length, returnCode.length);
            return resArray;
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
            return new int[]{(int) 0x03, (int) 0x90, (int) 0x00}; // OK
        } catch (IOException e) {
            e.printStackTrace();
            return new int[]{(int) 0x42, (int) 0x69}; // exception management
        }
    }

    private boolean isOK(int[] returnedCode) {
        return Arrays.equals(returnedCode, new int[]{(int) 0x03, (int) 0x90, (int) 0x00});
    }

    private byte[] intArrayToByteArray(int[] intArray) {
        byte[] res = new byte[intArray.length];
        for (int i = 0; i < intArray.length; i++) {
            res[i] = (byte) (intArray[i] & 0xFF);
        }
        return res;
    }

    private int[] byteArrayToIntArray(byte[] byteArray) {
        int[] res = new int[byteArray.length];
        for (int i = 0; i < byteArray.length; i++) {
            res[i] = byteArray[i];
        }
        return res;
    }

    private byte[] hexStringToByteArray(String s) {
        int len = s.length();
        byte[] data = new byte[len / 2];
        for (int i = 0; i < len; i += 2) {
            data[i / 2] = (byte) ((Character.digit(s.charAt(i), 16) << 4)
                    + Character.digit(s.charAt(i + 1), 16));
        }
        return data;
    }

    /**
     * Returns the content from the CC or NDEF file.
     *
     * @return
     */
    private byte[] getFileContent(int offset, int le) throws IOException {
        // TODO: 14/02/2019 check this
        String content = getFileAsString(selectedFile).substring(2 * offset, 2 * (offset + le));
        String[] array = content.replaceAll("..(?!$)", "$0 ").split(" ");
        return (hexStringToByteArray(content));

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