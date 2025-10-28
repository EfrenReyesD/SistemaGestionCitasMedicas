# GUIDs para Pruebas en Swagger

## IDs Fijos Precargados en el Sistema

Usa estos GUIDs para probar los endpoints que requieren un ID en Swagger.

---

## Pacientes

### Paciente 1: María González
```
11111111-1111-1111-1111-111111111111
```
- **Nombre**: María González
- **Email**: maria.gonzalez@email.com
- **Teléfono**: +34 600 123 456
- **Fecha de Nacimiento**: 1990-05-15

### Paciente 2: Carlos Rodríguez
```
22222222-2222-2222-2222-222222222222
```
- **Nombre**: Carlos Rodríguez
- **Email**: carlos.rodriguez@email.com
- **Teléfono**: +34 600 234 567
- **Fecha de Nacimiento**: 1979-08-22

---

## Doctores

### Doctor 1: Dr. Juan Pérez (Cardiología)
```
33333333-3333-3333-3333-333333333333
```
- **Nombre**: Dr. Juan Pérez
- **Email**: juan.perez@hospital.com
- **Cédula**: MED-12345
- **Especialidad**: Cardiología

### Doctor 2: Dra. Laura Sánchez (Pediatría)
```
44444444-4444-4444-4444-444444444444
```
- **Nombre**: Dra. Laura Sánchez
- **Email**: laura.sanchez@hospital.com
- **Cédula**: MED-67890
- **Especialidad**: Pediatría

---

## Citas

### Cita 1: María González con Dr. Juan Pérez
```
55555555-5555-5555-5555-555555555555
```
- **Paciente**: María González
- **Doctor**: Dr. Juan Pérez
- **Tipo**: Consulta General
- **Duración**: 30 minutos
- **Estado**: Programada

### Cita 2: Carlos Rodríguez con Dra. Laura Sánchez
```
66666666-6666-6666-6666-666666666666
```
- **Paciente**: Carlos Rodríguez
- **Doctor**: Dra. Laura Sánchez
- **Tipo**: Control Pediátrico
- **Duración**: 45 minutos
- **Estado**: Programada

### Cita 3: María González con Dr. Juan Pérez
```
77777777-7777-7777-7777-777777777777
```
- **Paciente**: María González
- **Doctor**: Dr. Juan Pérez
- **Tipo**: Revisión Cardiológica
- **Duración**: 30 minutos
- **Estado**: Programada

---

## Ejemplos de Uso en Swagger

### 1. Obtener un Paciente Específico
**GET** `/api/pacientes/11111111-1111-1111-1111-111111111111`

### 2. Obtener un Doctor Específico
**GET** `/api/doctores/33333333-3333-3333-3333-333333333333`

### 3. Obtener una Cita Específica
**GET** `/api/citas/55555555-5555-5555-5555-555555555555`

### 4. Ver Historial de Paciente
**GET** `/api/pacientes/11111111-1111-1111-1111-111111111111/historial`

### 5. Ver Agenda del Doctor
**GET** `/api/doctores/33333333-3333-3333-3333-333333333333/agenda?fecha=2025-01-15`

### 6. Reprogramar una Cita
**PUT** `/api/citas/55555555-5555-5555-5555-555555555555/reprogramar`

**Body:**
```json
"2025-02-10T14:00:00"
```

### 7. Cancelar una Cita
**PUT** `/api/citas/66666666-6666-6666-6666-666666666666/cancelar`

### 8. Agregar Nota Médica a una Cita
**POST** `/api/citas/55555555-5555-5555-5555-555555555555/nota`

**Body:**
```json
{
  "texto": "Paciente presenta mejoría notable",
  "diagnostico": "Presión arterial controlada"
}
```

---

## Formato de GUIDs

Los GUIDs deben ingresarse en Swagger exactamente como se muestran:
- **Con guiones**: `11111111-1111-1111-1111-111111111111`
- **Sin espacios adicionales**
- **Respetando mayúsculas/minúsculas** (aunque no es case-sensitive)

---

## Verificación Rápida

Para verificar que los datos están cargados:

1. **GET** `/api/pacientes` ? Debe devolver 2 pacientes
2. **GET** `/api/doctores` ? Debe devolver 2 doctores
3. **GET** `/api/citas` ? Debe devolver 3 citas
4. **GET** `/api/usuarios` ? Debe devolver 2 usuarios (Admin y Recepcionista)

---

## Copia Rápida (GUIDs solamente)

```
Paciente 1: 11111111-1111-1111-1111-111111111111
Paciente 2: 22222222-2222-2222-2222-222222222222
Doctor 1:   33333333-3333-3333-3333-333333333333
Doctor 2:   44444444-4444-4444-4444-444444444444
Cita 1:     55555555-5555-5555-5555-555555555555
Cita 2:     66666666-6666-6666-6666-666666666666
Cita 3:     77777777-7777-7777-7777-777777777777
```
