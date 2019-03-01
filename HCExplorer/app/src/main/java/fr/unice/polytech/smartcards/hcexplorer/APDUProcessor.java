package fr.unice.polytech.smartcards.hcexplorer;

import android.content.Context;

import org.apache.commons.io.IOUtils;

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

import static fr.unice.polytech.smartcards.hcexplorer.Utils.byteArrayToIntArray;
import static fr.unice.polytech.smartcards.hcexplorer.Utils.intArrayToByteArray;
import static fr.unice.polytech.smartcards.hcexplorer.Utils.intToByteArray;
import static fr.unice.polytech.smartcards.hcexplorer.Utils.twoBytesToInt;

/**
 * Created by user on 27/02/2019.
 */

public class APDUProcessor {

    private Context context;

    public static final String NDEF_FILE_NAME = "ndef_file.txt";
    public static final String CC_FILE_NAME = "cc_file.txt";
    public static final String CC_FILE_CONTENT = "000F20003B00340406E104080000";
    public static final int[] CC_FILE_CONTENT_BYTES = new int[]{0x00, 0x0F, 0x20, 0x00, 0x3B, 0x00, 0x34, 0x04, 0x06, 0xE1, 0x04, 0x08, 0x00, 0x00};
    public static final String NDEF_FILE_CONTENT = "0022D1021D5370910111550170617261676F6E2D72666964EE636F6D5101045400504944";
    public static final int[] NDEF_FILE_CONTENT_BYTES =
            new int[]{0x00, 0x22, 0xD1, 0x02, 0x1D, 0x53, 0x70, 0x91, 0x01, 0x11, 0x55, 0x01, 0x70, 0x61, 0x72, 0x61, 0x67, 0x6F, 0x6E, 0x2D, 0x72, 0x66, 0x69, 0x64, 0x2E, 0x63, 0x6F, 0x6D, 0x51, 0x01, 0x04, 0x54, 0x00, 0x50, 0x49, 0x44};

    private File ndefFile = null;
    private File ccFile = null;

    //        int[] ccContent = new int[]{0x00, 0x0F, 0x20, 0x00, 0x3B, 0x00, 0x34, 0x04, 0x06, 0xE1, 0x04, 0x00, 0x32, 0x00, 0x00};

    /**
     * E103h => CC file
     * 8101h => NDEF file
     */
    private File selectedFile;

    private int[] selectApplicationResult = null;
    private int[] selectFileResult = null;

    public APDUProcessor(Context context) {
        this.context = context;
        ccFile = createFile(CC_FILE_NAME, CC_FILE_CONTENT_BYTES);
        ndefFile = createFile(NDEF_FILE_NAME, NDEF_FILE_CONTENT_BYTES);
    }

    public APDUProcessor(File ccFile, File ndefFile) {
        this.ccFile = ccFile;
        this.ndefFile = ndefFile;
    }

    public int[] processCommandApdu(int[] apdu) {
        int cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new int[]{0x6E, 0x00}; // unknown CLA
        }
        int ins = apdu[1];
        // INS check
        if (ins == 0xA4) {
            // SELECT
            int p1 = apdu[2];
            if (p1 == 0x04) {
                // SELECT APPLICATION
                selectApplicationResult = selectApplication(apdu);
                return selectApplicationResult;
            } else if (p1 == 0x00) {
                // SELECT FILE
                if (isOK(selectApplicationResult)) {
                    selectFileResult = selectFile(apdu);
                    return selectFileResult;
                } else {
                    return new int[]{0x69, 0x86}; // etat non conforme
                }
            } else {
                return new int[]{0x6A, 0x86}; // incorrect P1/P2 SELECT
            }
        } else if (ins == 0xB0) {
            return isOK(selectFileResult) ? readBinary(apdu) : new int[]{0x69, 0x86}; // etat non conforme
        } else if (ins == 0xD6) {
            return isOK(selectFileResult) ? updateBinary(apdu) : new int[]{0x69, 0x86}; // etat non conforme
        } else {
            return new int[]{0x6D, 0x00}; // unknown INS
        }

    }

    private File createFile(String filename, int[] ints) {
//        String[] array = fileContent.replaceAll("..(?!$)", "$0 ").split(" ");
        byte[] bytes = new byte[ints.length];
        for (int i = 0; i < bytes.length; i++) {
            bytes[i] = (byte) (ints[i]);
        }
        File file = new File(context.getFilesDir(), filename);
        try (FileOutputStream fos = context.openFileOutput(file.getName(), Context.MODE_PRIVATE)) {
            fos.write(bytes);
        } catch (IOException e) {
            e.printStackTrace();
        }
        return file;
    }

    public static int[] selectApplication(int[] apdu) {
        int cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new int[]{0x6E, 0x00}; // unknown CLA
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
                        int[] validData = {0xD2, 0x76, 0x00, 0x00, 0x85, 0x01, 0x01};
                        if (Arrays.equals(data, validData)) {
                            int le = apdu[12];
                            if (le == 0x00) {
                                return new int[]{0x03, 0x90, 0x00}; // OK
                            } else {
                                return new int[]{0x6C, 0x00}; // incorrect Le
                            }
                        } else {
                            return new int[]{0x6A, 0x82}; // unknown AID/LID
                        }
                    } else {
                        return new int[]{0x67, 0x00}; // incorrect Lc
                    }
                } else {
                    return new int[]{0x6A, 0x86}; // incorrect P1/P2 SELECT
                }
            } else {
                return new int[]{0x6A, 0x86}; // incorrect P1/P2 SELECT
            }
        } else {
            return new int[]{0x6D, 0x00}; // unknown INS
        }
    }

    public int[] selectFile(int[] apdu) {
        int cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new int[]{0x6E, 0x00}; // unknown CLA
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
                    int[] ccData = {0xE1, 0x03};
                    if (Arrays.equals(data, ccData)) {
                        selectedFile = ccFile;
                        return new int[]{0x03, 0x90, 0x00}; // OK
                    } else {
                        selectedFile = ndefFile;
                        return new int[]{0x03, 0x90, 0x00}; // OK
                    }
                } else {
                    return new int[]{0x67, 0x00}; // incorrect Lc
                }
            } else {
                return new int[]{0x6A, 0x86}; // incorrect P1/P2 SELECT
            }
        } else {
            return new int[]{0x6D, 0x00}; // unknown INS
        }

    }

    public int[] readBinary(int[] apdu) {
        int cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new int[]{0x6E, 0x00}; // unknown CLA
        }
        int ins = apdu[1];
        // INS check
        if (ins != 0xB0) {
            return new int[]{0x6D, 0x00}; // unknown INS
        }
        int[] offset = {apdu[2], apdu[3]};
        int le = apdu[4];
        // TODO: 14/02/2019 check offset + le <= maxLe, sinon return 6C00
        try {
            int[] maxLe = getFileSubContent(3, 2);
            if (twoBytesToInt(offset[0], offset[1]) + le > twoBytesToInt(maxLe[0], maxLe[1])) {
                return new int[]{0x6C, 0x00};
            }
            int[] fileContent = getFileContent();
            int[] ccLen = selectedFile == ccFile ? byteArrayToIntArray(intToByteArray(fileContent.length + 1, 2)) : new int[0];
            int[] returnCode = new int[]{0x90, 0x00};
            int[] resArray = new int[fileContent.length + ccLen.length + returnCode.length];
            System.arraycopy(ccLen, 0, resArray, 0, ccLen.length);
            System.arraycopy(fileContent, 0, resArray, ccLen.length, fileContent.length);
            System.arraycopy(returnCode, 0, resArray, ccLen.length + fileContent.length, returnCode.length);
            return resArray;
        } catch (IOException e) {
            e.printStackTrace();
            return new int[]{0x42, 0x69}; // exception management
        }
    }

    public int[] updateBinary(int[] apdu) {
        int cla = apdu[0];
        // CLA check
        if (cla != 0x00) {
            return new int[]{0x6E, 0x00}; // unknown CLA
        }
        int ins = apdu[1];
        // INS check
        if (ins != 0xD6) {
            return new int[]{0x6D, 0x00}; // unknown INS
        }
        int[] offset = {apdu[2], apdu[3]};
        int lc = apdu[4];
        // TODO: 14/02/2019 check offset + lc <= maxLc, sinon return 6A87
        int[] maxLc = getFileSubContent(ccFile, 5, 2);
        if (twoBytesToInt(offset[0], offset[1]) + lc > twoBytesToInt(maxLc[0], maxLc[1])) {
            return new int[]{0x6A, 0x87};
        }
        int[] contentToWrite = Arrays.copyOfRange(apdu, 5, 5 + lc);
        int[] resToPrint = new int[contentToWrite.length];
        for (int i = 0; i < contentToWrite.length; i++) {
            resToPrint[i] = contentToWrite[i];
        }
        return writeToFile(resToPrint);
    }

    private int[] writeToFile2(int[] contentToWrite) {
        try (BufferedWriter bw = new BufferedWriter(new FileWriter(selectedFile))) {
            bw.write(new String(intArrayToByteArray(contentToWrite), Charset.forName("UTF-8")));
            return new int[]{0x03, 0x90, 0x00}; // OK
        } catch (IOException e) {
            e.printStackTrace();
            return new int[]{0x42, 0x69}; // exception management
        }
    }

    private int[] writeToFile(int[] contentToWrite) {
        try (FileOutputStream fos = context.openFileOutput(selectedFile.getName(), Context.MODE_PRIVATE)) {
            fos.write(intArrayToByteArray(contentToWrite));
            return new int[]{0x03, 0x90, 0x00}; // OK
        } catch (IOException e) {
            e.printStackTrace();
            return new int[]{0x42, 0x69}; // exception management
        }
    }

    private boolean isOK(int[] returnedCode) {
        return Arrays.equals(returnedCode, new int[]{0x03, 0x90, 0x00});
    }

    /**
     * Returns the content from the CC or NDEF file.
     *
     * @return
     */
    private int[] getFileContent() throws IOException {
        // TODO: 14/02/2019 check this
        return getFileAsIntArray(selectedFile);
    }

    /**
     * Returns a sub part the content from the CC or NDEF file.
     *
     * @return
     */
    private int[] getFileSubContent(int offset, int le) throws IOException {
        return getFileSubContent(selectedFile, offset, le);
    }

    /**
     * Returns a sub part the content from the CC or NDEF file.
     *
     * @return
     */
    private int[] getFileSubContent(File file, int offset, int le) {
        try {
            return Arrays.copyOfRange(getFileAsIntArray(file), offset, offset + le + 1);
        } catch (IOException e) {
            e.printStackTrace();
            return new int[2];
        }
    }

    private String getFileAsString2(File file) throws IOException {
        StringBuilder sb = new StringBuilder();
        BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(file)));
        String line;
        while ((line = br.readLine()) != null) {
            sb.append(line);
        }
        return sb.toString();
    }

    private int[] getFileAsIntArray(File file) throws IOException {
        try (FileInputStream in = context.openFileInput(file.getName())) {
            byte[] bytes = IOUtils.toByteArray(in);
            return Utils.byteArrayToIntArray(bytes);
        } catch (IOException e) {
            e.printStackTrace();
        }
        return null;
    }

}
