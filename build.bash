#!/usr/bin/env bash

systemctl --user stop adminlte
pushd AdminLte
rm -rf bin
dotnet publish --configuration Release
popd
rm -rf dist
cp -r AdminLte/bin/Release/netcoreapp3.1/publish dist
## TODO dirty hack maybe?
cp -r AdminLte/Views dist/
systemctl --user start adminlte
