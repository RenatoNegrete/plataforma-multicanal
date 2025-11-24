package com.example.proyecto.pagos.dto;

public class KafkaMessage {
    private String mail;
    private String transactionId;
    private String message;

    public KafkaMessage() {
    }

    public KafkaMessage(String mail, String transactionId, String message) {
        this.mail = mail;
        this.transactionId = transactionId;
        this.message = message;
    }

    public String getMail() {
        return mail;
    }
    public String getTransactionId() {
        return transactionId;
    }
    public String getMessage() {
        return message;
    }
}
