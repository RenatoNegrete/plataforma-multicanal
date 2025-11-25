package com.example.proyecto.ordenes.dto;

public class IntencionPagoRequest {

    private Long amount;
    private String currency;
    private String description;
    private String clientMail;

    public IntencionPagoRequest() {}

    public IntencionPagoRequest(Long amount, String currency, String description, String clientMail) {
        this.amount = amount;
        this.currency = currency;
        this.description = description;
        this.clientMail = clientMail;
    }

    public Long getAmount() {
        return amount;
    }

    public void setAmount(Long amount) {
        this.amount = amount;
    }

    public String getCurrency() {
        return currency;
    }

    public void setCurrency(String currency) {
        this.currency = currency;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getClientMail() {
        return clientMail;
    }

    public void setClientMail(String clientMail) {
        this.clientMail = clientMail;
    }
}
