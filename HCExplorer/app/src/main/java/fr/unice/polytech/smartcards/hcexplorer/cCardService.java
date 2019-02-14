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
 * Renvoyer un code d'erreur != 0x9000 dans les try/catch
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

    private byte[] selectApplicationResult = null;
    private byte[] selectFileResult = null;

    @Override
    public byte[] processCommandApdu(byte[] apdu, Bundle bundle) {
        return new byte[]{(byte) 0x90, (byte) 0x00}; // OK
    }

    @Override
    public void onDeactivated(int reason) {

    }

    @Override
    public void onCreate() {
        ccFile = new File(getFilesDir(), CC_FILE_NAME);
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

    public byte[] selectApplication(byte[] apdu) {
        byte cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new byte[]{(byte) 0x6E, (byte) 0x00}; // unknown CLA
        }
        // INS check
        byte ins = apdu[1];
        if (ins == 0xA4) { // SELECT
            // Getting P1 and P2
            byte p1 = apdu[2];
            byte p2 = apdu[3];
            if (p1 == 0x04) { // SELECT APPLICATION
                if (p2 == 0x00) {
                    byte lc = apdu[4];
                    if (lc == 0x07) {
                        byte[] data = Arrays.copyOfRange(apdu, 5, 5 + lc); // 5 + Lc = 12
                        byte[] validData = {(byte) 0xD2, (byte) 0x76, (byte) 0x00, (byte) 0x00, (byte) 0x85, (byte) 0x01, (byte) 0x01};
                        if (data == validData) {
                            byte le = apdu[13];
                            if (le == 0x00) {
                                return new byte[]{(byte) 0x90, (byte) 0x00}; // OK
                            } else {
                                return new byte[]{(byte) 0x6C, (byte) 0x00}; // incorrect Le
                            }
                        } else {
                            return new byte[]{(byte) 0x6A, (byte) 0x82}; // unknown AID/LID
                        }
                    } else {
                        return new byte[]{(byte) 0x67, (byte) 0x00}; // incorrect Lc
                    }
                } else {
                    return new byte[]{(byte) 0x6A, (byte) 0x86}; // incorrect P1/P2 SELECT
                }
            } else {
                return new byte[]{(byte) 0x6A, (byte) 0x86}; // incorrect P1/P2 SELECT
            }
        } else {
            return new byte[]{(byte) 0x6D, (byte) 0x00}; // unknown INS
        }

    }

    public byte[] selectFile(byte[] apdu) {
        if (!isOK(selectApplicationResult)) {
            return new byte[]{(byte) 0x69, (byte) 0x86}; // etat non conforme
        }
        byte cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new byte[]{(byte) 0x6E, (byte) 0x00}; // unknown CLA
        }
        // INS check
        byte ins = apdu[1];
        if (ins == 0xA4) { // SELECT
            // Getting P1 and P2
            byte p1 = apdu[2];
            byte p2 = apdu[3];
            if (p1 == 0x00 && p2 == 0x0C) {
                byte lc = apdu[4];
                if (lc == 0x02) {
                    byte[] data = Arrays.copyOfRange(apdu, 5, 5 + lc); // 5 + Lc = 7
                    byte[] ccData = {(byte) 0xE1, (byte) 0x03};
                    byte[] ndefData = {(byte) 0x81, (byte) 0x01};
                    if (data == ccData) {
                        selectedFile = ccFile;
                        return new byte[]{(byte) 0x90, (byte) 0x00}; // OK
                    } else if (data == ndefData) {
                        selectedFile = ndefFile;
                        return new byte[]{(byte) 0x90, (byte) 0x00}; // OK
                    } else {
                        return new byte[]{(byte) 0x6A, (byte) 0x82}; // unknown AID/LID
                    }
                } else {
                    return new byte[]{(byte) 0x67, (byte) 0x00}; // incorrect Lc
                }
            } else {
                return new byte[]{(byte) 0x6A, (byte) 0x86}; // incorrect P1/P2 SELECT
            }
        } else {
            return new byte[]{(byte) 0x6D, (byte) 0x00}; // unknown INS
        }

    }

    public byte[] readBinary(byte[] apdu) {
        if (!isOK(selectFileResult)) {
            return new byte[]{(byte) 0x69, (byte) 0x86}; // etat non conforme
        }
        byte cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new byte[]{(byte) 0x6E, (byte) 0x00}; // unknown CLA
        }
        byte ins = apdu[1];
        // INS check
        if (ins != 0xB0) {
            return new byte[]{(byte) 0x6D, (byte) 0x00}; // unknown INS
        }
        byte[] offset = {apdu[2], apdu[3]};
        byte le = apdu[4];
        // TODO: 14/02/2019 check offset + le <= maxLe, sinon return 6C00
        try {
            // TODO: 14/02/2019 also return 9000 (and display data)
            return getFileContent(offset[0] + offset[1], le);
        } catch (IOException e) {
            e.printStackTrace();
            return new byte[]{(byte) 0x42, (byte) 0x69}; // exception management
        }
    }

    public byte[] updateBinary(byte[] apdu) {
        if (!isOK(selectFileResult)) {
            return new byte[]{(byte) 0x69, (byte) 0x86}; // etat non conforme
        }
        byte cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new byte[]{(byte) 0x6E, (byte) 0x00}; // unknown CLA
        }
        byte ins = apdu[1];
        // INS check
        if (ins != 0xD6) {
            return new byte[]{(byte) 0x6D, (byte) 0x00}; // unknown INS
        }
        byte[] offset = {apdu[2], apdu[3]};
        byte lc = apdu[4];
        // TODO: 14/02/2019 check offset + lc <= maxLc, sinon return 6A87
        byte[] contentToWrite = Arrays.copyOfRange(apdu, 5, 5 + lc);
        return writeToFile(contentToWrite);
    }

    private byte[] writeToFile(byte[] contentToWrite) {
        try {
            new BufferedWriter(new FileWriter(selectedFile)).write(new String(contentToWrite, Charset.forName("UTF-8")));
            return new byte[]{(byte) 0x90, (byte) 0x00}; // OK
        } catch (IOException e) {
            e.printStackTrace();
            return new byte[]{(byte) 0x42, (byte) 0x69}; // exception management
        }
    }

    private boolean isOK(byte[] returnedCode) {
        return returnedCode == new byte[]{(byte) 0x90, (byte) 0x00};
    }

    /**
     * Returns the content from the CC or NDEF file.
     *
     * @return
     */
    private byte[] getFileContent(int offset, int le) throws IOException {
        // TODO: 14/02/2019 check this
        return getFileAsString(selectedFile).substring(offset, offset + le).getBytes(Charset.forName("UTF-8"));
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