const fs = require('fs');
const path = require('path');
const crypto = require('crypto');

const distRoot = path.join(__dirname, 'dist');
const distDirs = fs.readdirSync(distRoot).filter(name => fs.statSync(path.join(distRoot, name)).isDirectory());

if (distDirs.length === 0) {
  console.error('❌ Nenhuma pasta encontrada em /dist. Você rodou o ng build?');
  process.exit(1);
}

// Agora acessa /dist/nome/browser
const projectDist = path.join(distRoot, distDirs[0], 'browser');
if (!fs.existsSync(projectDist)) {
  console.error(`❌ Pasta ${projectDist} não encontrada. Verifique se o build está correto.`);
  process.exit(1);
}

const files = fs.readdirSync(projectDist);
const mainJsFile = files.find(f => /^main-.*\.js$/.test(f));

if (!mainJsFile) {
  console.error('❌ Arquivo main-*.js não encontrado em:', projectDist);
  process.exit(1);
}

// Gera hash SHA-256
const buffer = fs.readFileSync(path.join(projectDist, mainJsFile));
const hash = crypto.createHash('sha256').update(buffer).digest('hex');

// Gera version.json
const version = {
  hash,
  file: mainJsFile,
  generatedAt: new Date().toISOString()
};

fs.writeFileSync(path.join(projectDist, 'version.json'), JSON.stringify(version, null, 2));
console.log('✅ version.json criado com sucesso em:', projectDist);
