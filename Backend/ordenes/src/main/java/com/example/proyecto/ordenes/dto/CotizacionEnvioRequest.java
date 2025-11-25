package com.example.proyecto.ordenes.dto;

import java.util.List;

public class CotizacionEnvioRequest {
    private String email;
    private List<Package> packages;
    private String description;
    private Integer contentValue;
    private Integer codValue;
    private Boolean includeGuideCost;
    private String codPaymentMethod;
    private Location origin;
    private Location destination;

    public CotizacionEnvioRequest() {
        // Valores por defecto quemados
        this.description = "descripción";
        this.contentValue = 12000;
        this.codValue = 12000;
        this.includeGuideCost = false;
        this.codPaymentMethod = "cash";
        
        // Paquete por defecto
        Package defaultPackage = new Package();
        defaultPackage.setWeight(1);
        defaultPackage.setHeight(10);
        defaultPackage.setWidth(5);
        defaultPackage.setLength(15);
        this.packages = List.of(defaultPackage);
        
        // Origen por defecto
        this.origin = new Location();
        this.origin.setDaneCode("11001000");
        this.origin.setAddress("Calle 98 62-37");
        
        // Destino por defecto
        this.destination = new Location();
        this.destination.setDaneCode("11001000");
        this.destination.setAddress("Carrera 46 # 93 - 45");
    }

    // Getters y setters
    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }

    public List<Package> getPackages() { return packages; }
    public void setPackages(List<Package> packages) { this.packages = packages; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public Integer getContentValue() { return contentValue; }
    public void setContentValue(Integer contentValue) { this.contentValue = contentValue; }

    public Integer getCodValue() { return codValue; }
    public void setCodValue(Integer codValue) { this.codValue = codValue; }

    public Boolean getIncludeGuideCost() { return includeGuideCost; }
    public void setIncludeGuideCost(Boolean includeGuideCost) { this.includeGuideCost = includeGuideCost; }

    public String getCodPaymentMethod() { return codPaymentMethod; }
    public void setCodPaymentMethod(String codPaymentMethod) { this.codPaymentMethod = codPaymentMethod; }

    public Location getOrigin() { return origin; }
    public void setOrigin(Location origin) { this.origin = origin; }

    public Location getDestination() { return destination; }
    public void setDestination(Location destination) { this.destination = destination; }
}
