# BurrowMadness

# Last Build
https://drive.google.com/file/d/1fcr8WASmgdF9bXEYPRGGY3j28qpT8kBU/view?usp=sharing

# Propuesta del juego
Burrow Madness! es el nuevo juego de Kortrex Game Studio que se lanzará este 2019
para dispositivos móviles. Se trata de una aventura que te llevará a diversos 
planetas de la galaxia Agrulds, en donde deberás explorar, recolectar minerales
y luchar contra las múltiples criaturas espaciales que encontrarás en tu 
camino. Lánzate a la aventura, vende tus preciosos materiales y equipa tu 
máquina con las armas más potentes de toda la galaxia, atrévete a llegar
a las profundidades de los planetas, saquea las mazmorras y conviertete en
un verdadero aventurero interplanetario.

# Tutorial Para Testing
Para la versiòn actual:
- Utiliza el joystick izquierdo para moverte por el mundo, 
los movimientos horizontales hacen que tu maquina navege
- En caso de que te encuentres con tierra no te detengas,
 puedes excavar y forjar tu propio camino.
- los puntos rojos representan los enemigos, te perseguiran
si nte encuentran.
- Disparales a los enemigos usando el joystick derecho, puedes
presionarlo para disparar al mas cercano o apuntarles a tu gusto.
- descubre y recolecta minerales que encuentres por todo el planeta 
excavando hacia ellos.
- Cuidado con las caidas de grandes alturas, pueden dañar tu
màquina. 


# Elementos Característicos
- Joystick de movimiento ubicado a la izquierda de la pantalla.
- Joystick de disparo a la derecha de la pantalla.
- Generación procedural de chunks para la construcción de cada mundo.
- uso de chunks prefabricados para la construcción de cada mundo.
- La construcción de cada mundo asocia la generación procedural para asegurar
un entorno extenso y usa los chunks prefabricados en ciertos puntos del mapa
que se convierten en hotspots o puntos de interés como mazmorras o landscapes.
- Minerales diferentes para la construcción del mundo.
- Inventario con actualización en tiempo real para almacenar minerales extraidos.
- Destrucción de tiles para simular excavación.
- Destrucción de tiles de minerales a diferente ritmo para simular la dureza del mineral.
- Sistema de disparo con apuntado mixto.
- Enemigos con inteligencia artificial de exploración en 2D.
- Sistema de barras de vida con representación en el UI para personaje principal y enemigos.
- Enemigos que atacan y restan vida correctamente.
- Daño por caidas calculado por altura.
- Sistema de combustible para el funcionamiento de la máquina.
- Sistema de monetización por medio de anuncios cada x tiempo (1 minuto en la versión actual)

# Últimos avances
- Se implementaron dos formas de disparo funcionales: Autoaim al enemigo más cercano / Directed Aim 
a preferencia del usuario.
- herramienta de editor para crear chunks prefabricados como mazmorras o landscapes.

# Elementos Pendientes
- Sistema de enemigos diferentes con atributos variables.
- Sistema de feedback auditivos / sonidos en general.
- Sistema de guardado de progreso.
- Arte y animación definitiva.
- Sistema de intercambio de minerales por divisa debil del juego
- Inclusión y uso de divisa fuerte dentro del juego.
- Sistema de visualización y selección de mundos.
- Sistema de armas que incluye 4 tipos de armas diferentes con atributos de caracteristica variables 
(mejorables) a cambio de una divisa debil del juego.

# Bugs Vigentes
- las misiones funcionan en la versión ejecutable de pc pero no en el apk para móviles.
- La navegaciòn sin usar el vuelo se dificulta por la fricciòn que genera el suelo.




# Entrega Beta Domingo 12 de Mayo


# Recursos De Terceros

## Simple UI de Unity (Para el User Interface Del juego) 
       - Se Usaron Los Botones PNG
       - Se Usaron los Simple Vector Icons
       - Se Usó FredokaOne-Regular (Fuente del paquete Simple UI)

## Basic Joystick Pack de Fenerax Studios  (Para los joysticks del juego) 
       - Se Implementó el fixed Joystick del paquete
       - Se usaron 2 BackgroundSprites 
       - Se usaron 2 Handle Sprites
       - Se usaron eventos para comunicar los inputs del joystick con las acciones del jugador.

## Priority Queue de BlueRaja (Optimizacion del pathfinder de los enemigos)
       - Utilización de la librería PriorityQueue
       - Referencias a FastPriorityQueue

## Unity Ads y Textmesh pro de Unity (Uso básico)
       - Textmesh para inlcuir texto en las interfaces 
       - Unity Ads para incluir Ads (duh) entre sesiones y monetizar en el futuro lanzamiento.

## Arte Galaxy Map Background de Freepik
## Uso de Prop de Kenney
       


# Recursos Propios

# Arte

## Sprites de Armas
   * Escopeta Pixel Art
   * Cañon Pixel Art
   * Ametralladora Pixel Art
   * Pistolaa Pixel Art
   * 4 Máscaras para recolorizar cada una de las armas.

## Sprites de Enemigos
   * 4 enemigos en 128 x 128 (2 enemigos implementados)
   * Brazos por separado de los enemigos para animación de estos (versión a posteriori)
   
## Sprites de Personaje
   * Sprite básico de personaje
   * Animación en 8 Frames de propulsor de personaje
   * Animación en 3 Frames de taladro de personaje
   
## Tiles Mundo
   * Spritesheet de todos los tiles necesarios para la construcción de los mundos
   * Capa RGB sobre el spritesheet de Tiles para cambiar los colores de acuerdo al mundo
   * Mapa de Normales de los tiles de los spritesheet
   * Shader RGB Mask
   
## Scriptable Objects

   * Se usaron 4 diferentes con el mismo fin de determinar los tiles gráficos que se deben usar en cada situación que se presente conforme se construye y destruye el mundo.
   
## Librerías nativas de C

  * Se usan para traducir los mundos generados a XML
  * Se usan para traducir de XML a objetos dentro de los archivos del juego.
  
## Scripts de Editor

  * 3 Scripts de Editor hechos por el programador en jefe, se utilizan para manipular los mundos desde el editor, tanto para crearlos     como guardarlos a mano.
   
   
   

