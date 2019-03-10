package fr.unice.polytech.smartcards.hcexplorer;

import android.nfc.cardemulation.HostApduService;
import android.os.Bundle;

import java.util.Arrays;

/**
 * Service waiting for interaction with the reader, and sending back its response.
 */

public class cCardService extends HostApduService {

    private APDUProcessor apduProcessor;

    @Override
    public byte[] processCommandApdu(byte[] apdu, Bundle bundle) {
        System.out.println("Received APDU : "+ Utils.hexPrint(apdu));
        int[] apduUnsigned = Utils.byteArrayToIntArray(apdu);
        byte[] result = Utils.intArrayToByteArray(apduProcessor.processCommandApdu(apduUnsigned));
        System.out.println("Send response : "+ Utils.hexPrint(result));
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