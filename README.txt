Prerequisite Requisite Requirements

For this code to work you need to have adb.exe installed on your computer.

ADB is the Andriod Debug Bridge (okay, technically I know A is for Android and B is for Bridge, I like D for Debug, 
but you can also use Deathly)

The official site for getting this file is: https://developer.android.com/studio/index.html#downloads 
(look in the Get Just the Command Line Tools area)

You can also directly download the 25.2.3 version here: https://dl.google.com/android/repository/tools_r25.2.3-windows.zip

Unzip the content and run the andriod.bat script.

I was able to do a fairly minimal install.

You'll also need to enabled developers options on your phone 
(per instructions on the cozmo sdk site (http://cozmosdk.anki.com/docs/adb.html)


	Tap seven (7) times on the Build Number listed under Settings -> About Phone.
	Then, under Settings -> Developer Options, enable USB debugging.

In the code example (which you've pulled from this repository) you'll need to alter the constructor's content for VMMain.  The line
reads:

	mConnection = new CozConnection(); 

Add the full path and filename as an argument to the constructor:

	mConnection = new CozConnection(@"c:\myfolder\mysubfolder\mysubsubfolder\adb.exe");
