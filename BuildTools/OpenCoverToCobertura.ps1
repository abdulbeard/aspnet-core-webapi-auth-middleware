Write-Host (Get-Item -Path ".\" -Verbose).FullName;
.\OpenCoverToCoberturaConverter.exe -Wait -NoNewWindow -ArgumentList "-input:'..\OpenCover.xml'" -output:"..\Cobertura.xml" -sources:"..\";