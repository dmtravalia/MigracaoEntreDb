# MigracaoEntreDb

Realiza a copia dos registros de X em X tempo de um tabela entre um banco SqlServer(origem) para um MySQL(destino).
Para o controle dos logs está utilizando Event Viewer Windows para guardar as informações de erro e um tabela no MySQL para visualização do funcionamento do serviço.

## Exemplo de Tabela Utilizada no Desenvolvimento

Exemplo da tabela utilizada no desenvolvido do serviço.

```sql
CREATE TABLE tb_origem (
	NumeroOrdemServico int NULL,
	DataAbertura datetime NULL,
	TipoServico varchar(100) NULL,
	TipoCsvNome varchar(100) NULL,
	Placa varchar(100) NULL,
	NumeroNotaFiscal int NULL,
	NumeroNFSE varchar(100) NULL,
	ValorBrutoNotaFiscal int NULL,
	ValorLiquidoNotaFiscal int NULL,
	ValorLiquidoServico int NULL,
	ISSRetido int NULL,
	ValorISS decimal(18,2) NULL,
	RetencaoIR decimal(18,2) NULL,
	RetencaoPIS decimal(18,2) NULL,
	RetencaoCOFINS decimal(18,2) NULL,
	RetencaoCS decimal(18,2) NULL,
	Situacao int NULL,
	CpfCnpj varchar(100) NULL,
	NomeRazaoSocial varchar(100) NULL,
	Portaria varchar(100) NULL,
	SubGrupo varchar(100) NULL,
	ProjetoSisLitIdentificacao varchar(100) NULL,
	TipoLaudoNome varchar(100) NULL,
	Chassi varchar(100) NULL,
	NotaFiscalCancelada int NULL,
	DataEmissao datetime NULL,
	DataVencimento datetime NULL
);
```

## Tabela de Log's de Atividades

Criada para visualização do funcionamento do serviço através da inserção da informação da quantidade de linhas que foram lida no banco do SqlServer e a quantidade de linhas que forma inseridas no MySQL.

```sql
CREATE TABLE log_service_migracao_entre_db (
	DataExecucao datetime NULL,
	Mensagem varchar(500) NULL
);
```