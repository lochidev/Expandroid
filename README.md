# Espanso compatible app for android. X-platform expansions
  <p align="center">
    <a href="https://github.com/lochidev/TextComparePro/issues">Report Bug</a>
    ·
    <a href="https://github.com/lochidev/TextComparePro/issues">Request Feature</a>
    ·
    <a href="https://github.com/lochidev/TextComparePro/releases">Releases</a>
    ·
    <a href="https://github.com/lochidev/TextComparePro/blob/master/examples/config.yml">Example config</a>
  </p>

Send custom messages with a trigger. Want to quickly type out the current date in a specific format? Or do you want your emojis to replace your triggers? You can do it all cross platform with espanso but now on android too with this app!

<p align="center">
    <a href="https://apt.izzysoft.de/fdroid/index/apk/com.dingleinc.texttoolspro"><img src="https://gitlab.com/IzzyOnDroid/repo/-/raw/master/assets/IzzyOnDroid.png" alt="Get it on IzzyOnDroid" height="80"></a>
    <a href="https://play.google.com/store/apps/details?id=com.dingleinc.texttoolspro"><img src="https://cdn.rawgit.com/steverichey/google-play-badge-svg/master/img/en_get.svg" height="80"></a>
    <a href="https://github.com/lochidev/TextComparePro/releases/latest"><img src="https://raw.githubusercontent.com/andOTP/andOTP/master/assets/badges/get-it-on-github.png" height="80"></a>
  </p>

# Disclaimer
I am now a full time student doing science stuff and I dont have much time to develop this app. It is too much work and takes a lot of hours that could've helped with my studies and it has very little pay off.  So my plan for the app is to update every 3 months or so when I have free time.

# Notes 
Espanso configuration YML files will take a few tries to parse correctly. Try removing some matches, and make sure it's compliant with the YML specs. Some working examples are provided below for your convenience to copy and paste. Please also note that only the following extensions are supported -> date, clipboard, random and echo. Finally, note that not all espanso/rust chrono date time formats are supported. Supported formats are,
- %Y, %m, %b, %B, %h, %d, %e, %a, %A, %j, %w, %u, %D, %F, %H, %I, %p, %M, %S, %R, %T, %r

You can further customize date time formats by referring to the C# DateTime.ToString() method documentation from Microsoft.

Clipboard extension will not work on android 10 or higher due to security measures introduced by google. (Note: I have not tested this app on android versions below 12)

# Build

CLI build instructions -> https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli

# Consider donating for work already done if you found this app useful! 💙
<a href="https://www.buymeacoffee.com/lochi" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-yellow.png" alt="Buy Me A Coffee" height="41" width="174"></a>

BTC - bc1q0tv7u3yngq3xpmwlu4gzv8rnez27pv3xcsk6t8

LTC - ltc1qjphn23kql69c0ul6qu88sdsf09qxceswt78yey

