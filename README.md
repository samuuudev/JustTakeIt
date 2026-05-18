# JustTakeIt - Proyecto TFG

## Descripción

JustTakeIt es un videojuego de plataformas 2D desarrollado en Unity centrado en el movimiento mediante recoil. El jugador utiliza el retroceso de su arma como herramienta principal de movilidad para superar obstáculos, evitar enemigos y optimizar rutas.

El proyecto ha sido desarrollado como Trabajo de Fin de Grado (TFG) aplicando conocimientos de programación, diseño de videojuegos, interfaces, sistemas online y arquitectura modular.

---

# Características principales

* Movimiento basado en recoil
* Plataformas 2D centradas en precisión
* Sistema de puntuación y cronómetro
* Leaderboard online mediante Supabase
* Persistencia local de configuración
* HUD y menús interactivos
* Compatibilidad con teclado y ratón

---

# Tecnologías utilizadas

* Unity Engine
* C#
* Supabase
* SoundsGood
* TextMeshPro
* Git / Plastic SCM

---

# Arquitectura del proyecto

El proyecto utiliza una arquitectura modular basada en componentes MonoBehaviour de Unity.

Scripts principales:

* GameManager
* PlayerMovement
* CameraController
* Enemy
* Coin
* SupabaseManager
* LeaderboardUI

---

# Funcionalidades implementadas

## Gameplay

* Movimiento horizontal
* Salto
* Sistema recoil
* Gestión de munición
* Enemigos patrulla
* Sistema de muerte y reinicio

## Sistemas

* HUD dinámico
* Cronómetro
* Ranking online
* Persistencia de audio
* Gestión de escenas

---

# Estructura del proyecto

```plaintext
Assets/
├── Scenes/
├── Scripts/
├── Prefabs/
├── Audio/
├── UI/
├── Materials/
└── Sprites/
```

---

# Instalación y ejecución

1. Clonar el repositorio
2. Abrir el proyecto con Unity 6
3. Cargar la escena principal
4. Ejecutar desde el editor o generar una build para Windows

---

# Objetivos del proyecto

* Desarrollar un videojuego funcional
* Implementar mecánicas de movimiento avanzadas
* Integrar sistemas online
* Aplicar arquitectura modular
* Aprender el flujo completo de desarrollo de videojuegos

---

# Autor

Samuel Quintano Molina
Virutal Bool Studios

---

# Licencia

Proyecto desarrollado con fines educativos y académicos.
