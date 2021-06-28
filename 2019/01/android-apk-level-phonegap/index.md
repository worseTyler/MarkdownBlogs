
![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2019/01/android-apk-level-phonegap/images/AndroidTargetAPK-300x225.png)

## Attain Hassle-Free Acceptance of Your Google Play Application with PhoneGap.

**\*UPDATE: PhoneGap is dead. Consider [some alternatives](https://apppresser.com/phonegap-build-is-dead-here-are-some-alternatives/).**

I’ve been working on a mobile task management application that is currently in [Open Beta on Android](https://play.google.com/store/apps/details?id=com.faustware.firsttask), and as December was waning, I knew I needed to add a few months to the beta expiration date that was currently set to expire on December 31, 2018. After extending the expiration, I compiled the app and found that Google Play would not accept it.

This particular app was written using TypeScript with PhoneGap. PhoneGap is a service by Adobe that lets you write your application once in JavaScript, sending the source up to PhoneGap where you can then compile it for Android, iOS and Windows Phone.

### Google Requires A Minimum Target APK Level

The reason Google would not accept the app is because I hadn't specified a minimum target APK level yet. I double-clicked the config file in the application to open the settings dialog. As you can see (below), it fields for minimum, maximum and target _API_ levels but no settings for the _APK_ level.

![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2019/01/android-apk-level-phonegap/images/Tom5.png) As I was searching the web for a way to set the target APK level, I found a number of articles that said to create a preference called `android-targetSdkVersion` with a value of 26. To do this, I right-clicked the `config.xml` file and selected View Code from the context menu.

![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2019/01/android-apk-level-phonegap/images/2.config_api_menu-.png)

I added the new preference alongside other preferences in the config file, as shown below.

![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2019/01/android-apk-level-phonegap/images/3.config_api_2.png)

After doing so, PhoneGap happily built the application with the same incorrect target APK level of 14 that it had before I set the preference. Since it was less than the minimum requirement of 26, Google still would not accept the build. For a while, this was quite confusing because the target APK level was correct (as you can see in the illustration above).

### Solve the Problem by Locating the Proper `config.xml` File

It turned out, there was a second `config.xml` file located in the www folder.

![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2019/01/android-apk-level-phonegap/images/4.www_menu.png)

This `config.xml` file contained just a few settings, and when I added the preference here, PhoneGap finally used it to build the application correctly for Android.

![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2019/01/android-apk-level-phonegap/images/5.config_apk.png)

The bottom-line is that when you need to set the target APK level for PhoneGap, make sure that you modify the correct config file which is the one located in the www folder.

Have you used PhoneGap before? How has it simplified your app? Comment below.
