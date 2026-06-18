Write-Host "Limpando diretórios bin, obj, out e TestResults..." -ForegroundColor Yellow
Get-ChildItem -Path . -Recurse -Directory -Filter "bin" -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path . -Recurse -Directory -Filter "obj" -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path . -Recurse -Directory -Filter "TestResults" -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
if (Test-Path out) { Remove-Item -Recurse -Force out -ErrorAction SilentlyContinue }
Write-Host "Limpeza concluída com sucesso!" -ForegroundColor Green
