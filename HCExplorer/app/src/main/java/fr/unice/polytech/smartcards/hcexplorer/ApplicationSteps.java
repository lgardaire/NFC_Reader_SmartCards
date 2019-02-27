package fr.unice.polytech.smartcards.hcexplorer;

import java.util.Arrays;

/**
 * Created by user on 27/02/2019.
 */

public class ApplicationSteps {
    public static int[] selectApplication(int[] apdu) {
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


}
