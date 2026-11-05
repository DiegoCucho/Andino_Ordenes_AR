# Andino_Ordenes_AR — Escenario 4 · Las 36 órdenes (AR)

Proyecto ANDES-SIM (Banco Andino). Datos 100 % ficticios, basados en el robo al Banco de Bangladesh (2016).

## Qué hay en el repositorio
- `Assets/Resources/ordenes.json`: las 36 órdenes (US$951 M): 30 bloqueadas, 4 pagadas, 1 «Fandation», 1 no reenviada.
- `Assets/Scripts/Core`: `GameManager` (modo Ataque/Defensa, `RegisterEvidence`, `ApplyDefense`, `EndScenario`), `ScenarioData`, `ModeToggleButton`.
- `Assets/Scripts/Ordenes`: `OrderData`, `OrderTarget`, `RuleEngine`, `TypoDetector`, `InventoryUI`.

## Montaje en Unity (pasos)
1. Instalar Vuforia (`com.ptc.vuforia.engine`), pegar la licencia Basic en la ARCamera (**no subirla al repo**).
2. Crear la escena `Ordenes_AR`: ARCamera de Vuforia + un objeto vacío `GameManager` con el script `GameManager`.
3. Crear un `ScenarioData` (Assets > Create > ANDES-SIM > ScenarioData) y asignarlo al GameManager.
4. Crear la base de datos de Vuforia con 6 a 8 imágenes (3 estrellas o más) y añadir `ImageTarget_Orden_01..08`.
5. Prefab `Ficha_Orden`: un Cube con 5 TextMeshPro 3D (id, monto, destino, beneficiario, estado + motivos).
6. En cada ImageTarget añadir `OrderTarget`, poner su `orderId` (p. ej. `ORD-05` = «Fandation») y arrastrar la ficha y los textos.
7. En el `DefaultObserverEventHandler` de cada ImageTarget: *On Target Found* → `OrderTarget.OnFound`, *On Target Lost* → `OrderTarget.OnLost`.
8. Canvas: un botón con `ModeToggleButton` y un panel con `InventoryUI` (TextMeshPro UGUI).

## Mapa de órdenes recomendado para los marcadores
`ORD-01` pagada · `ORD-02` pagada · `ORD-05` «Fandation» · `ORD-06` no reenviada · `ORD-07` bloqueada · `ORD-20` bloqueada · `ORD-36` bloqueada

## Marcadores (carpeta `Marcadores/`)
Imágenes `ORD_01, 02, 03, 04, 05, 06, 07, 20` (.jpg, 1400x1000) para subir a Vuforia Target Manager, y `Marcadores_Ordenes.pdf` para imprimir (una por página).

## Funciones del escenario (RF y extras)
- `OrderTarget`: ficha 3D con animación de entrada en C# al detectar el marcador (OnTargetFound).
- `OrdenesPanel`: RF-02 clasificar la orden (bloqueada, pagada o retenida) con aciertos; RF-03 panel que explica qué validación detuvo «Fandation»; pantalla final.
- `InventoryUI`: totales de lo escaneado o del caso completo (36 órdenes) con el botón «Inventario completo».
- `CreditosUI`: pantalla de créditos (editar el campo *Creditos* del Inspector con los nombres del equipo).
- Menú `ANDES-SIM > Agregar clasificación, final y créditos` crea toda la interfaz en `Canvas_UI`.
