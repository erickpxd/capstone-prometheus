# Prometheus V2 - Capstone

## Sobre o Projeto

O **Prometheus V2** é uma proposta de evolução arquitetural do sistema **Prometheus V1**, desenvolvida como parte do Capstone da disciplina de Arquitetura de Software.

O projeto busca modernizar a arquitetura do sistema legado, preservando suas funcionalidades principais e melhorando atributos de qualidade como **modularidade**, **escalabilidade**, **manutenibilidade** e **facilidade de evolução**.

A proposta adota uma abordagem incremental, permitindo que o sistema evolua sem a necessidade de uma substituição completa da aplicação existente.

---

## Objetivos

* Modernizar a arquitetura do Prometheus V1.
* Reduzir o acoplamento entre módulos.
* Melhorar a organização do sistema por domínios.
* Facilitar manutenção e evolução do software.
* Documentar as principais decisões arquiteturais.
* Aplicar boas práticas de Arquitetura de Software utilizando o modelo C4.

---

## Arquitetura

O Prometheus V2 utiliza uma arquitetura baseada em **Monólito Modular**, organizada por domínios de negócio.

Os principais módulos considerados são:

* Usuários
* Cursos
* Conteúdo
* Notas
* Vídeos
* Portal Web

Essa organização favorece alta coesão, baixo acoplamento e possibilita uma futura migração para uma arquitetura distribuída, caso necessário.

---

## Estrutura do Repositório

```text
.
├── docs/
│   ├── diagrams/
│   ├── adr/
│   └── mockups/
├── src/
├── README.md
└── CONTRIBUTING.md
```

---

## Documentação

Toda a documentação do projeto encontra-se na **Wiki** do GitLab.

**Wiki:** https://gitlab.com/jala-university1/cohort-3/PT.CSAR-364.GA.T2.26.M1/SB/erick.lima/capstone-prometheus/-/wikis/home

A Wiki contém:

* Visão Geral do Projeto
* Análise de Lacunas
* Requisitos
* Arquitetura
* Diagramas C4
* Trade-offs
* Decisões Arquiteturais
* Cronograma
* Gestão de Riscos
* Entregáveis

---

## Tecnologias

* C#
* ASP.NET Core
* SQL Server
* Entity Framework Core
* REST APIs
* Git
* GitLab
* Mermaid
* PlantUML

---

## Organização

| Item         | Local              |
| ------------ | ------------------ |
| Código-fonte | Repositório GitLab |
| Documentação | Wiki do GitLab     |
| Diagramas    | `docs/diagrams`    |
| ADRs         | `docs/adr`         |

---

## Contribuição

Para manter o projeto organizado:

* Utilize branches para cada funcionalidade ou documentação.
* Abra Merge Requests para revisão antes da integração.
* Atualize a Wiki sempre que houver mudanças arquiteturais relevantes.
* Documente decisões importantes utilizando ADRs.

---

## Equipe

Projeto desenvolvido para o **Capstone da disciplina de Arquitetura de Software**, com foco no reprojeto arquitetural do sistema Prometheus V1 para o Prometheus V2.
