set windows-shell := ["powershell", "-NoProfile", "-Command"]

# Limpa o projeto removendo pastas bin, obj, out e resultados de teste
clean:
    @powershell -NoProfile -ExecutionPolicy Bypass -File scripts/clean.ps1

# Cria um backup zip limpo do projeto, excluindo lixo e pastas temporárias
backup:
    @powershell -NoProfile -ExecutionPolicy Bypass -File scripts/backup.ps1
