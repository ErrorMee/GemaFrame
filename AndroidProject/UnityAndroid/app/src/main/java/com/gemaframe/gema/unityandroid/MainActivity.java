package com.gemaframe.gema.unityandroid;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.widget.Toast;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

public class MainActivity extends UnityPlayerActivity
{
    private Toast mToast;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
    }

    public void showToast(final String text)
    {
        new Handler(Looper.getMainLooper()).post(new Runnable() {
            @Override
            public void run() {
                if(mToast == null)
                {
                    mToast = Toast.makeText(MainActivity.this,text,Toast.LENGTH_SHORT);
                }else
                {
                    mToast.setText(text);
                }
                mToast.show();
            }
        });
    }

    public String getNowTime()
    {
        long time = System.currentTimeMillis();
        return new SimpleDateFormat("yyyy年MM月dd日 HH:mm:ss", Locale.CHINESE).format(new Date(time));
    }

    public void CallUnityFunc(String uObjName,String methodName)
    {
        UnityPlayer.UnitySendMessage(uObjName, methodName,"0");
    }
}
