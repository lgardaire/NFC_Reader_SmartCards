package fr.unice.polytech.smartcards.hcexplorer;

import android.nfc.cardemulation.HostApduService;
import android.os.Bundle;

/**
 * Created by user on 07/02/2019.
 */

public class cCardService extends HostApduService {

    @Override
    public byte[] processCommandApdu(byte[] bytes, Bundle bundle) {
        return new byte[] {(byte)0x90, (byte)0x00};
    }

    @Override
    public void onDeactivated(int reason) {

    }
}
