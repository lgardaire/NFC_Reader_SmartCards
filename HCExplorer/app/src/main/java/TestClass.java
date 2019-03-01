import android.content.Intent;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.math.BigInteger;
import java.util.Arrays;

import fr.unice.polytech.smartcards.hcexplorer.APDUProcessor;
import fr.unice.polytech.smartcards.hcexplorer.Utils;

/**
 * Created by user on 27/02/2019.
 */

public class TestClass {

    public static void main(String[] args) {
        byte[] res = new byte[2];
        byte[] i = BigInteger.valueOf(0x100).toByteArray();
        if(i.length == 2){
            res = i;
        } else {
            res[0] = 0;
            res[1] = i[0];
        }

        System.out.println(Arrays.toString(res));


        File ccFile = new File(APDUProcessor.CC_FILE_NAME);
        File ndefFile = new File(APDUProcessor.NDEF_FILE_NAME);

        try (BufferedWriter bwCC = new BufferedWriter(new FileWriter(ccFile));
             BufferedWriter bwNDEF = new BufferedWriter(new FileWriter(ndefFile))) {
            bwCC.write(APDUProcessor.CC_FILE_CONTENT);
            bwNDEF.write(APDUProcessor.NDEF_FILE_CONTENT);
        } catch (IOException e) {
            e.printStackTrace();
        }

        APDUProcessor processor = new APDUProcessor(ccFile, ndefFile);
        exec(processor, new int[]{0x00, 0xA4, 0x04, 0x00, 0x07, 0xD2, 0x76, 0x00, 0x00, 0x85, 0x01, 0x01, 0x00});
        exec(processor, new int[]{0x00, 0xA4, 0x00, 0x0C, 0x02, 0xE1, 0x03});
        exec(processor, new int[]{0x00, 0xB0, 0x00, 0x00, 0x0F});
        exec(processor, new int[]{0x00, 0xA4, 0x00, 0x0C, 0x02, 0x00, 0x04});
        exec(processor, new int[]{0x00, 0xB0, 0x00, 0x00, 0x03});
        exec(processor, new int[]{0x00, 0xD6, 0x00, 0x00, 0x07, 0xD1, 0x00, 0x04, 0x62, 0x69, 0x74, 0x65});
    }

    private static void exec(APDUProcessor processor, int[] apdu){
        byte[] res = Utils.intArrayToByteArray(processor.processCommandApdu(apdu));
        System.out.println(Arrays.toString(Utils.hexPrint(res)));
    }
}
