#!/usr/bin/env bash

systemctl stop adminlte
sleep 5
rm -rf AdminLte/{obj,bin}
rm -rf dist
git pull origin master
pushd AdminLte
dotnet publish --configuration Release
popd
cp -r AdminLte/bin/Release/netcoreapp3.1/publish dist
## TODO dirty hack maybe?
cp -r AdminLte/Views dist/
rm -rf dist/wwwroot/uploads
ln -s /srv/uploads dist/wwwroot/uploads
systemctl start adminlte
