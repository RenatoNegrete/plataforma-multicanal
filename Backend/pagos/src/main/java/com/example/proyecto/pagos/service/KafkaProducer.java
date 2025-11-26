package com.example.proyecto.pagos.service;

import org.springframework.kafka.core.KafkaTemplate;
import org.springframework.stereotype.Service;

import com.example.proyecto.pagos.dto.KafkaMessage;

@Service
public class KafkaProducer {

    private final KafkaTemplate<String, KafkaMessage> kafkaTemplate;

    public KafkaProducer(KafkaTemplate<String, KafkaMessage> kafkaTemplate) {
        this.kafkaTemplate = kafkaTemplate;
    }

    public void send(KafkaMessage dto) {
        kafkaTemplate.send("notifications-payment", dto);
    }

}

