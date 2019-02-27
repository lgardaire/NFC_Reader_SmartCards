import java.util.Arrays;

import fr.unice.polytech.smartcards.hcexplorer.ApplicationSteps;

/**
 * Created by user on 27/02/2019.
 */

public class TestClass {

    public static void main(String[] args) {
        int[] sel = ApplicationSteps.selectApplication(new int[]{0x00, 0xA4, 0x04, 0x00, 0x07, 0xD2, 0x76, 0x00, 0x00, 0x85, 0x01, 0x01, 0x00});
        System.out.println(Arrays.toString(sel));
    }
}
