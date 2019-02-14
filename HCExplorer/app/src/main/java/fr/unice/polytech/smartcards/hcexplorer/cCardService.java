package fr.unice.polytech.smartcards.hcexplorer;

import android.nfc.cardemulation.HostApduService;
import android.os.Bundle;

import java.io.File;
import java.util.Arrays;

/**
 * Created by user on 07/02/2019.
 * Renvoyer un code d'erreur != 0x9000 dans les try/catch
 */

public class cCardService extends HostApduService {

    private File ndefFile;

    @Override
    public byte[] processCommandApdu(byte[] apdu, Bundle bundle) {
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
                                // TODO: 14/02/2019 SELECT APPLICATION
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
            } else if (p1 == 0x00 && p2 == 0x0C) {
                byte lc = apdu[4];
                if (lc == 0x02) {
                    byte[] data = Arrays.copyOfRange(apdu, 5, 5 + lc); // 5 + Lc = 7
                    byte[] ccData = {(byte) 0xE1, (byte) 0x03};
                    if (data == ccData) {
                        // TODO: 14/02/2019 CC SELECT
                    } else {
                        // TODO: 14/02/2019 NDEF SELECT -> data = file ID
                    }
                } else {
                    return new byte[]{(byte) 0x67, (byte) 0x00}; // incorrect Lc
                }
                // TODO: 14/02/2019 SELECT FILE
            } else {
                return new byte[]{(byte) 0x6A, (byte) 0x86}; // incorrect P1/P2 SELECT
            }
        } else if (ins == 0xB0 || ins == 0xB1) {
            // TODO: 14/02/2019 READ BINARY
        } else if (ins == 0xD6 || ins == 0xD7) {
            // TODO: 14/02/2019 UPDATE BINARY
        } else {
            return new byte[]{(byte) 0x6D, (byte) 0x00}; // unknown INS
        }

        return new byte[]{(byte) 0x90, (byte) 0x00};
    }

    @Override
    public void onDeactivated(int reason) {

    }

    @Override
    public void onCreate() {
        String ndefFileName = "ndef_file.txt";
        for (File file : getApplicationContext().getFilesDir().listFiles()) {
            if (file.getName().equals(ndefFileName)) {
                ndefFile = file;
                break;
            }
        }
        if (ndefFile == null) {
            ndefFile = new File(getApplicationContext().getFilesDir(), ndefFileName);
        }
    }

    public void selectApplication() {
    }

    public void selectFile() {

    }

    public void readBinary() {

    }

    public void updateBinary() {

    }

}