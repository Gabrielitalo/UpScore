const fs = require('fs');
const path = './src/app/configs/version.ts';
const crypto = require('crypto');

// Gera um hash curto (6 caracteres)
const hash = crypto.randomBytes(4).toString('hex');

const content = `// Gerado automaticamente a cada build
export const APP_VERSION = '${hash}';
`;

fs.writeFile(path, content, (err) => {
  if (err) {
    console.error('❌ Erro ao gerar versão:', err);
    process.exit(1);
  } else {
    console.log('✅ Versão (hash) atualizada para:', hash);
  }
});
