const express = require('express');
const sql = require('mssql');
const path = require('path');

const app = express();

app.use(express.urlencoded({ extended: true }));
app.use(express.json());

app.use(express.static(__dirname));

const dbConfig = {
    server: 'localhost',
    port: 1433,
    database: 'SapatoVelhoDB',
    user: 'sa',
    password: 'SenhaDoPim123',
    options: {
        encrypt: false,
        trustServerCertificate: true
    }
};

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'cadastro.html'));
});

app.post('/cadastrar', async (req, res) => {

    const {
        Nome,
        Datadenascimento,
        CPF,
        Email,
        Senha,
        Endereco,
        CEP,
        Cidade
    } = req.body;

    try {

        const pool = await sql.connect(dbConfig);

        await pool.request()
            .input('nome', sql.VarChar(150), Nome)
            .input('dataNasc', sql.VarChar(50), Datadenascimento)
            .input('cpf', sql.VarChar(20), CPF)
            .input('email', sql.VarChar(100), Email)
            .input('senha', sql.VarChar(100), Senha)
            .input('endereco', sql.VarChar(200), Endereco)
            .input('cep', sql.VarChar(20), CEP)
            .input('cidade', sql.VarChar(100), Cidade)
            .query(`
                INSERT INTO Usuarios
                (
                    Nome,
                    DataNascimento,
                    CPF,
                    Email,
                    Senha,
                    Endereco,
                    CEP,
                    Cidade
                )
                VALUES
                (
                    @nome,
                    @dataNasc,
                    @cpf,
                    @email,
                    @senha,
                    @endereco,
                    @cep,
                    @cidade
                )
            `);

        return res.send(`
            <h1>Conta criada com sucesso!</h1>
            <a href="/">Voltar</a>
        `);

    } catch (erro) {

        console.error('ERRO:', erro);

        if (erro.number === 2627) {
            return res.status(400).send(`
                <h1>CPF ou E-mail já cadastrado!</h1>
            `);
        }

        return res.status(500).send(`
            <h1>Erro ao salvar no banco de dados</h1>
            <pre>${erro.message}</pre>
        `);
    }
});

app.listen(3000, () => {
    console.log('Servidor rodando em http://localhost:3000');
});