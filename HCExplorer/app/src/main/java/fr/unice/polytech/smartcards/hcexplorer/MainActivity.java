package fr.unice.polytech.smartcards.hcexplorer;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import org.apache.commons.io.IOUtils;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStreamReader;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        setText();
        findViewById(R.id.refresh).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                setText();
            }
        });
    }

    private void setText() {
        ((TextView) findViewById(R.id.textView)).setText(getNdefFileContent());
    }

    public String getNdefFileContent() {
        try (FileInputStream in = openFileInput(APDUProcessor.NDEF_FILE_NAME)) {
            byte[] bytes = IOUtils.toByteArray(in);
            return new String(bytes);
        } catch (IOException e) {
            e.printStackTrace();
        }
        return "";
    }
}
