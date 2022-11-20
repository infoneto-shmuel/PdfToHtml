del Packages\PdfRecognizerLibrary.*
rmdir /s /q %userprofile%\.nuget\Packages\PdfRecognizerLibrary
nuget restore PdfRepresentation.sln
MSBuild.exe PdfRepresentation.sln /property:Configuration=Debug
git push
git add -A
git commit -a --allow-empty-message -m ''
git push
