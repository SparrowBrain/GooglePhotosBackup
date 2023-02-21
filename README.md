# GooglePhotosBackup

The app is partly written using ChatGPT.

It's a backup tool to download all photos in the cloud and store locally. In case these photos already exist, they will be skipped.

Requires Oauth Client ID to generated. The credentials should be saved as `client_secret.json` and set to `Copy if newer` in properties. `client_secret.json` should never be commited, since it contains secrets.

`appsettings.example.json` should be copied to `appsettings.json` and set to `Copy if newer`. Customize `appsettings.json` as per personal needs.

Abandoned due to ridiculous Google APIs functioanlity. You cannot download original files easily (especially videos). Switched to OneDrive instead as it supports local backup from the get go. No coding required.