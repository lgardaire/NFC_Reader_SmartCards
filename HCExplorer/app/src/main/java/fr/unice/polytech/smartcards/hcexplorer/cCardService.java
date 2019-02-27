package fr.unice.polytech.smartcards.hcexplorer;

import android.nfc.cardemulation.HostApduService;
import android.os.Bundle;

/**
 * Created by user on 07/02/2019.
 */

public class cCardService extends HostApduService {

    private APDUProcessor apduProcessor;

    @Override
    public byte[] processCommandApdu(byte[] apdu, Bundle bundle) {
        int[] apduUnsigned = new int[apdu.length];
        for (byte i = 0; i < apdu.length; i++) {
            apduUnsigned[i] = apdu[i] & 0xFF;
        }
        return Utils.intArrayToByteArray(apduProcessor.processCommandApdu(apduUnsigned));
    }

    @Override
    public void onDeactivated(int reason) {

    }

    @Override
    public void onCreate() {
        apduProcessor = new APDUProcessor(getApplicationContext());
    }

}