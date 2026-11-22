// Servidor estático do Front-End (http://localhost:3000).
// O banco de dados agora é acessado somente pela API .NET (SapatoVelho.Api).
const express = require('express');
const path = require('path');

const app = express();

app.use(express.static(__dirname));

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'main.html'));
});

app.listen(3000, () => {
    console.log('Site rodando em http://localhost:3000');
});
