dotnet publish ./src/KordanorsCatacomb/KordanorsCatacomb.vbproj -o ./pub-linux -c Release --sc -r linux-x64
dotnet publish ./src/KordanorsCatacomb/KordanorsCatacomb.vbproj -o ./pub-windows -c Release --sc -r win-x64
butler push pub-windows thegrumpygamedev/kordanors-catacomb:windows
butler push pub-linux thegrumpygamedev/kordanors-catacomb:linux
git add -A
git commit -m "shipped it!"