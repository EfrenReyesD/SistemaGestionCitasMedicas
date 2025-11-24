# GUIDs de Prueba - Swagger

## Datos Precargados en Base de Datos

### Pacientes
```
11111111-1111-1111-1111-111111111111  # María González (maria.gonzalez@email.com)
22222222-2222-2222-2222-222222222222  # Carlos Rodríguez (carlos.rodriguez@email.com)
```

### Doctores
```
33333333-3333-3333-3333-333333333333  # Dr. Juan Pérez - Cardiología
44444444-4444-4444-4444-444444444444  # Dra. Laura Sánchez - Pediatría
```

### Citas
```
55555555-5555-5555-5555-555555555555  # María + Dr. Pérez (Consulta General)
66666666-6666-6666-6666-666666666666  # Carlos + Dra. Sánchez (Pediatría)
77777777-7777-7777-7777-777777777777  # María + Dr. Pérez (Cardiología)
```

---

## Ejemplos Rápidos en Swagger

### Consultar datos existentes
```
GET /api/pacientes/11111111-1111-1111-1111-111111111111
GET /api/doctores/33333333-3333-3333-3333-333333333333
GET /api/citas/55555555-5555-5555-5555-555555555555
```

### Ver agenda y historial
```
GET /api/doctores/33333333-3333-3333-3333-333333333333/agenda?fecha=2025-01-15
GET /api/pacientes/11111111-1111-1111-1111-111111111111/historial
```

### Modificar citas
```
PUT /api/citas/55555555-5555-5555-5555-555555555555/reprogramar
Body: "2025-02-10T14:00:00"

PUT /api/citas/66666666-6666-6666-6666-666666666666/cancelar
```

### Agregar nota médica
```
POST /api/citas/55555555-5555-5555-5555-555555555555/nota?idDoctorAutor=33333333-3333-3333-3333-333333333333
Body: {
  "texto": "Paciente presenta mejoría notable",
  "diagnostico": "Presión arterial controlada"
}
```

---

## Verificar Datos
```
GET /api/pacientes   ? Debe retornar 2 pacientes
GET /api/doctores    ? Debe retornar 2 doctores
GET /api/citas       ? Debe retornar 3 citas
