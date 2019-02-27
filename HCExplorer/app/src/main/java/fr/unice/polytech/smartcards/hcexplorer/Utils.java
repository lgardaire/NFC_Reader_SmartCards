package fr.unice.polytech.smartcards.hcexplorer;

/**
 * Created by user on 27/02/2019.
 */

public class Utils {
    public static byte[] intArrayToByteArray(int[] intArray) {
        byte[] res = new byte[intArray.length];
        for (int i = 0; i < intArray.length; i++) {
            res[i] = (byte) (intArray[i] & 0xFF);
        }
        return res;
    }

    public static int[] byteArrayToIntArray(byte[] byteArray) {
        int[] res = new int[byteArray.length];
        for (int i = 0; i < byteArray.length; i++) {
            res[i] = byteArray[i];
        }
        return res;
    }

    public static byte[] hexStringToByteArray(String s) {
        int len = s.length();
        byte[] data = new byte[len / 2];
        for (int i = 0; i < len; i += 2) {
            data[i / 2] = (byte) ((Character.digit(s.charAt(i), 16) << 4)
                    + Character.digit(s.charAt(i + 1), 16));
        }
        return data;
    }

}
