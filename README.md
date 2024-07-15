# Teste Técnico para 67Bits

Este projeto é um teste técnico para a empresa 67Bits. Ele envolve um sistema de movimentação de personagem em Unity com mecânicas de carregar e socar objetos, além de uma interface de usuário para controle de dinheiro e upgrades.

## Instalação

1. Clone o repositório para a sua máquina local:
    ```
    git clone https://github.com/seu-usuario/seu-repositorio.git
    ```

2. Abra o projeto no Unity.

## Scripts

### `PlayerCollider.cs`

Este script gerencia as colisões do jogador com objetos que podem ser carregados e socados.

### `PlayerInputHandler.cs`

Este script gerencia a entrada do jogador usando o sistema de entrada do Unity.

### `UIController.cs`

Este script controla a UI, incluindo a atualização do texto de dinheiro e a funcionalidade do botão de compra.

### `CharacterMovement.cs`

Este script controla o movimento do jogador e a lógica de carregar objetos.

## Como Usar

- Ao colidir com objetos etiquetados como `Punchable`, o jogador os socará.
- Ao colidir com objetos etiquetados como `Carriable`, o jogador começará a carregá-los.
- O jogador pode vender objetos carregados ao colidir com uma área de venda etiquetada como `Drop`.
- O dinheiro ganho é atualizado na UI, e o jogador pode usar o botão de compra para aumentar a capacidade de carga.

---

# Technical Test for 67Bits

This project is a technical test for the company 67Bits. It involves a character movement system in Unity with mechanics for carrying and punching objects, as well as a user interface for money control and upgrades.

## Installation

1. Clone the repository to your local machine:
    ```
    git clone https://github.com/your-username/your-repository.git
    ```

2. Open the project in Unity.

## Scripts

### `PlayerCollider.cs`

This script manages player collisions with objects that can be carried and punched.

### `PlayerInputHandler.cs`

This script manages player input using the Unity input system.

### `UIController.cs`

This script controls the UI, including updating the money text and the buy button functionality.

### `CharacterMovement.cs`

This script controls the player's movement and the logic for carrying objects.

## How to Use

- When colliding with objects tagged as `Punchable`, the player will punch them.
- When colliding with objects tagged as `Carriable`, the player will start carrying them.
- The player can sell carried objects by colliding with a selling area tagged as `Drop`.
- The earned money is updated in the UI, and the player can use the buy button to increase the carrying capacity.
