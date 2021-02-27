#!/usr/bin/env bash

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
