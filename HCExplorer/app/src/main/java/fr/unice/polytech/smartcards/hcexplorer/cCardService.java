package fr.unice.polytech.smartcards.hcexplorer;

import android.nfc.cardemulation.HostApduService;
import android.os.Bundle;

import java.util.Arrays;

/**
 * Created by user on 07/02/2019.
 */

public class cCardService extends HostApduService {

    private APDUProcessor apduProcessor;

    @Override
    public byte[] processCommandApdu(byte[] apdu, Bundle bundle) {
        System.out.println("Received APDU : "+ Arrays.toString(Utils.hexPrint(apdu)));
        int[] apduUnsigned = Utils.byteArrayToIntArray(apdu);
        byte[] result = Utils.intArrayToByteArray(apduProcessor.processCommandApdu(apduUnsigned));
        System.out.println("Send response : "+ Arrays.toString(Utils.hexPrint(result)));
        return result;
    }

    @Override
    public void onDeactivated(int reason) {

    }

    @Override
    public void onCreate() {
        apduProcessor = new APDUProcessor(getApplicationContext());
    }

}