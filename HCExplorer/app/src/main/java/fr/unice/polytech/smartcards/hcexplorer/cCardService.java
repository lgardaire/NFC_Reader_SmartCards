package fr.unice.polytech.smartcards.hcexplorer;

import android.nfc.cardemulation.HostApduService;
import android.os.Bundle;

import java.io.File;

/**
 * Created by user on 07/02/2019.
 */

public class cCardService extends HostApduService {

    private File ndefFile;

    @Override
    public byte[] processCommandApdu(byte[] bytes, Bundle bundle) {
        return new byte[] {(byte)0x90, (byte)0x00};
    }

    @Override
    public void onDeactivated(int reason) {

    }

    @Override
    public void onCreate() {
        String ndefFileName = "ndef_file.txt";
        for (File file : getApplicationContext ().getFilesDir().listFiles()){
            if(file.getName().equals(ndefFileName)){
                ndefFile = file;
                break;
            }
        }
        if(ndefFile == null){
            ndefFile = new File(getApplicationContext().getFilesDir(), ndefFileName);
        }
    }

    public void selectApplication(){
        
    }

}