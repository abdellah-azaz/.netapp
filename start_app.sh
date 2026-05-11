#!/bin/bash

# Configuration des chemins
BACKEND_DIR="/home/abdellah/.gemini/antigravity/scratch/fastapi"
FRONTEND_DIR="/home/abdellah/.gemini/antigravity/scratch/MonAppMultiplateforme"

# 1. Démarrer le Backend FastAPI
echo "Démarrage du Backend FastAPI..."
cd "$BACKEND_DIR"
if [ -f "venv/bin/activate" ]; then
    source venv/bin/activate
fi
# On lance le backend en arrière-plan
python3 main.py > backend.log 2>&1 &
BACKEND_PID=$!

# On s'assure que le backend est tué quand le script s'arrête
trap "kill $BACKEND_PID" EXIT

# 2. Démarrer le Frontend .NET
echo "Démarrage du Frontend .NET (Avalonia)..."
cd "$FRONTEND_DIR"
# On utilise dotnet run pour le mode développement
dotnet run

# Le 'trap' s'occupe de tuer le backend quand on ferme l'app .NET
