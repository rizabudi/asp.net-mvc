#!/usr/bin/env bash

export DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/1001/bus
systemctl --user stop adminlte
rm -rf AdminLte/{obj,bin}
rm -rf dist
pushd AdminLte
dotnet publish --configuration Release
popd
cp -r AdminLte/bin/Release/netcoreapp3.1/publish dist
## TODO dirty hack maybe?
cp -r AdminLte/Views dist/
systemctl --user start adminlte
rm -rf dist/wwwroot/uploads
ln -s /srv/uploads dist/wwwroot/uploads
