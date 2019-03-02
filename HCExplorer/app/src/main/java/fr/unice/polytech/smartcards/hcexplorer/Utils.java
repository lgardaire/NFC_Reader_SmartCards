package fr.unice.polytech.smartcards.hcexplorer;

import java.util.Arrays;

/**
 * Utility methods used in conversions
 */

public class Utils {

    /**
     * Converts an array of integers in an array of bytes.
     * @param intArray array to convert
     * @return the converted array
     */
    public static byte[] intArrayToByteArray(int[] intArray) {
        byte[] res = new byte[intArray.length];
        for (int i = 0; i < intArray.length; i++) {
            res[i] = (byte) (intArray[i] & 0xFF);
        }
        return res;
    }

    /**
     * Converts an array of bytes in an array of integers.
     * @param byteArray array to convert
     * @return the converted array
     */
    public static int[] byteArrayToIntArray(byte[] byteArray) {
        int[] res = new int[byteArray.length];
        for (int i = 0; i < byteArray.length; i++) {
            if(byteArray[i] < 0){
                res[i] = byteArray[i] & 0xFF;
            } else {
                res[i] = byteArray[i];
            }
        }
        return res;
    }

    /**
     * Converts an integer in an array of bytes.
     * @param i the integer to convert
     * @param nbBytes the length of the converted number, in bytes
     * @return the converted value
     */
    public static byte[] intToByteArray(int i, int nbBytes) {
        byte[] result = new byte[nbBytes];
        for (int j = 0, k = nbBytes; j < nbBytes; j++, k--) {
            result[j] = (byte) (i >> 8 * (k - 1));
        }
        return result;
    }

    /**
     * Converts a two bytes array in an integer
     * @param b0 first byte
     * @param b1 second byte
     * @return the converted value
     */
    public static int twoBytesToInt(int b0, int b1) {
        return (int) (b0 * Math.pow(16, 2) + b1);
    }

    /**
     * Returns an array of bytes in a String representing the same array, but with hex values.
     * This method is mainly used for debug.
     * eg : [144, 0] -> ["0x90", "0x00"]
     * @param array array to convert
     * @return the array in a String format, each number written in hexadecimal
     */
    public static String hexPrint(byte[] array){
        String[] beautify = new String[array.length];
        for(int i = 0; i < array.length; i++){
            beautify[i] = String.format("0x%02X", array[i]);
        }
        return Arrays.toString(beautify);
    }

}
