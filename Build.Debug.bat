if exist ..\WhenTheVersion\Packages\ (
  mklink /j Packages ..\WhenTheVersion\Packages 
) else (
  md Packages
)
del Packages\PdfRepresantation.*
rmdir /s /q %userprofile%\.nuget\Packages\PdfRepresantation
nuget restore PdfRepresantation.sln
MSBuild.exe PdfRepresantation.sln /property:Configuration=Debug 
copy "PdfRepresantation\bin\Debug\PdfRepresantation.1.0.0.nupkg" Packages\
copy "PdfRepresantation\bin\Debug\PdfRepresantation.1.0.0.symbols.nupkg" Packages\
git add -A
git commit -a --allow-empty-message -m ''
git push
