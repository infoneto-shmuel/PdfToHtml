IF NOT DEFINED Configuration SET Configuration=Release
MSBuild.exe PdfRepresentation.sln -t:restore -p:RestorePackagesConfig=true
MSBuild.exe PdfRepresentation.sln -m /property:Configuration=Debug
git push
git add -A
git commit -a --allow-empty-message -m ''
git push
