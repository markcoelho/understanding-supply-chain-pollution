void generateQuestions() { 
    //  questions and their answers 
    questions[0][0] = "How often do you order food delivery in a week?";
    questions[0][1] = "0 times"; 
    questions[0][2] = "1-2 times"; 
    questions[0][3] = "3-4 times"; 
    questions[0][4] = "5+ times"; 
    
    questions[1][0] = "How many products do you order online per month?";
    questions[1][1] =  "0-1"; 
    questions[1][2] =  "1-5"; 
    questions[1][3] =  "5-10";
    questions[1][4] =  "10+"; 
    
    questions[2][0] = "How many liters of bottled water do you drink in a week?";
    questions[2][1] = "0"; 
    questions[2][2] = "1-5"; 
    questions[2][3] = "6-9"; 
    questions[2][4] = "9+"; 

    questions[3][0] = "How many pairs of shoes do you buy in a year?";
    questions[3][1] = "0-2"; 
    questions[3][2] = "3-5"; 
    questions[3][3] = "6-8"; 
    questions[3][4] = "9+"; 

    questions[4][0] = "How often do you buy pre-packaged snacks in a week?";
    questions[4][1] = "0-1 times"; 
    questions[4][2] = "2-3 times"; 
    questions[4][3] = "4-5 times"; 
    questions[4][4] = "6+ times"; 

    questions[5][0] = "How many electronics do you buy in a year?";
    questions[5][1] = "0"; 
    questions[5][2] = "1-2"; 
    questions[5][3] = "2-5"; 
    questions[5][4] = "5+"; 

    questions[6][0] = "How many pieces of clothing do you buy in a month?";
    questions[6][1] = "0-1"; 
    questions[6][2] = "2-5"; 
    questions[6][3] = "6-10"; 
    questions[6][4] = "10+";

}

int numbquestions = 7;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.HashMap;

boolean showStartScreen = true;

void saveAnswerToFile(int questionNumber, int answerNumber) {
    BufferedWriter writer = null;
    try {
        //file fora da build \/
        //File file = new File(sketchPath("unity project/Assets/StreamingAssets/quiz/answer.txt"));
        //file na build \/
        File file = new File(sketchPath("digital mirror_Data/StreamingAssets/quiz/answer.txt"));
        
        writer = new BufferedWriter(new FileWriter(file, false)); // Override mode to delete previous content

        String output = calculatePollution(questionNumber, answerNumber);
        writer.write(output + "\n");

    } catch (IOException e) {
        e.printStackTrace();
    } finally {
        try {
            if (writer != null) writer.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
//1 pedaco de papel = 10 gramas papel
//1 caixa cartao = 200 gramas cartao
//1 pedaco plastico = 20 gramas plastico
//1 garrafa agua = 30 gramas
//1 cup = 240 gramas
//1 bag = 20 gramas
String calculatePollution(int questionNumber, int answerNumber) {
    HashMap<Integer, String[]> pollutionData = new HashMap<>();
    pollutionData.put(0, new String[]{"4320 paper", "2400 cup", "2400 plasticbag", "384 box"});
    pollutionData.put(1, new String[]{"600 box", "1500 plastic"});
    pollutionData.put(2, new String[]{"2400 bottle", "516 plastic"});
    pollutionData.put(3, new String[]{"37 box", "75 paper"});
    pollutionData.put(4, new String[]{"3000 plastic"});
    pollutionData.put(5, new String[]{"25 box", "150 paper"});
    pollutionData.put(6, new String[]{"800 paper", "150 box", "45 plastic"});
    

    float[][] answerMultiplier = {
        {0f, 1.5f, 3.5f, 5.5f}, // question 0
        {0.5f, 3f, 7.5f, 10.5f}, // question 1 (averages for the ranges)
        {0f, 3f, 7.5f, 10.5f}, // question 2
        {1f, 4f, 7f, 9f}, // question 3
        {0.5f, 2.5f, 4.5f, 6f}, // question 4
        {0f, 1f, 2f, 3.5f}, // question 5
        {0.5f, 3f, 8f, 10.5f} // question 6
        
    };

    float multiplier = answerMultiplier[questionNumber][answerNumber];
    StringBuilder result = new StringBuilder();

    for (String item : pollutionData.get(questionNumber)) {
        String[] parts = item.split(" ");
        int quantity = Integer.parseInt(parts[0]);
        String type = parts[1];
        int adjustedQuantity = Math.round(quantity * multiplier);
        result.append(adjustedQuantity).append(" ").append(type).append("\n");
    }

    return result.toString();
}



int questionIndex = 0;
String[][] questions = new String[numbquestions][5];
boolean[] userAnswers = new boolean[numbquestions];
boolean quizCompleted = false;
Checkbox[] checkboxes = new Checkbox[4];
boolean waitForFeedback = false;
boolean answerSelected = false;
int selectedAnswerIndex = -1;
float fadeValue = 0;
boolean fadingOut = false;
boolean fadingIn = false;
color bgColor = color(40, 44, 52);
color textColor = color(255);
color checkboxColor = color(70, 130, 180);
Button startButton;
Button confirmButton;

PImage[] backgrounds = new PImage[7];

void setup() {
    size(1000, 500);
  //fullScreen(2);//PICK SCREEN TO OPEN ON
    textFont(createFont("Arial", 18));
    clearAnswerFile(); // Limpar o arquivo answer.txt no início
    generateQuestions();
    
    for (int i = 0; i < 4; i++) {
        float checkboxX = width / 2 - 300 + i * 200; // Ajustar o espaçamento horizontal
        checkboxes[i] = new Checkbox(checkboxX, height / 2 - 80); // Ajustar o espaçamento vertical
    }

    startButton = new Button(width / 2 - 50, height / 2 + 50, 100, 50, "Start");
    confirmButton = new Button(width / 2 - 50, height / 2 + 150, 100, 50, "Confirm");

    // Carregar imagens de fundo
    backgrounds[0] = loadImage("images/food.jpg");
    backgrounds[1] = loadImage("images/delivery.jpg");
    backgrounds[2] = loadImage("images/water.jpg");
    backgrounds[3] = loadImage("images/shoes.jpg");
    backgrounds[4] = loadImage("images/snacks.jpg");
    backgrounds[5] = loadImage("images/electronics.jpg");
    backgrounds[6] = loadImage("images/clothing.jpg");
}

void exit() {
   clearAnswerFile(); // Limpar o arquivo answer.txt ao sair
   super.exit(); // Chama o método exit da superclasse para finalizar o programa
}


void clearAnswerFile() {
    BufferedWriter writer = null;
    try {
        //file fora da build \/
        //File file = new File(sketchPath("unity project/Assets/StreamingAssets/quiz/answer.txt"));
        //file na build \/
        File file = new File(sketchPath("digital mirror_Data/StreamingAssets/quiz/answer.txt"));
        
        writer = new BufferedWriter(new FileWriter(file, false)); // Override mode to delete previous content
        writer.write(""); // Escrever uma string vazia para limpar o arquivo
    } catch (IOException e) {
        e.printStackTrace();
    } finally {
        try {
            if (writer != null) writer.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}



void draw() {
    background(bgColor);

    if (showStartScreen) {
        showStartScreenScreen();
    } else if (quizCompleted) {
        showThankYouMessage();
    } else {
        showQuestion(questionIndex);
        for (Checkbox cb : checkboxes) {
            cb.display();
        }
        if (answerSelected) {
            confirmButton.display();
        }
    }
}


void showStartScreenScreen() {
  fill(textColor);
  textSize(24);
  textAlign(CENTER, CENTER);
  text("This is a quiz about how much pollution you generate in your life.\nAre you ready to start?", width / 2, height / 2 - 50);
  startButton.display();
}


void showQuestion(int index) {
    tint(255, 60); // Define a opacidade da imagem para 50%
    image(backgrounds[index], 0, 0, width, height); // Desenha a imagem de fundo com a opacidade ajustada
    noTint(); // Reseta a opacidade para os desenhos seguintes
    
    fill(textColor);
    textSize(24); // Aumentar o tamanho do texto para melhor visibilidade
    textAlign(CENTER, CENTER); // Centralizar o texto horizontalmente e verticalmente
    text(questions[index][0], width / 2, height / 4); // Centralizar o texto da pergunta
    
    textSize(18);
    for (int i = 1; i <= 4; i++) {
        float answerX = width / 2 - 300 + (i - 1) * 200;
        fill(textColor);
        textAlign(CENTER, CENTER); // Centralizar o texto horizontalmente e verticalmente
        text(questions[index][i], answerX, height / 2 + 70); // Ajustar a posição vertical do texto para mais espaçamento
        checkboxes[i - 1].y = height / 2 + 40; // Ajustar a posição vertical das checkboxes
        checkboxes[i - 1].display(); // Desenhar as checkboxes
    }
}


void mousePressed() {
    if (quizCompleted || waitForFeedback) return;

    if (showStartScreen) {
        if (startButton.isClicked(mouseX, mouseY)) {
            showStartScreen = false;
        }
        return;
    }

    for (int i = 0; i < 4; i++) {
        if (checkboxes[i].isChecked(mouseX, mouseY)) {
            selectedAnswerIndex = i;
            answerSelected = true;
            for (Checkbox cb : checkboxes) {
                cb.isFilled = (cb == checkboxes[i]);
            }
            break;
        }
    }

    if (answerSelected && confirmButton.isClicked(mouseX, mouseY)) {
        userAnswers[questionIndex] = true;
        saveAnswerToFile(questionIndex, selectedAnswerIndex); // Save answer to file
        answerSelected = false;
        selectedAnswerIndex = -1;
        questionIndex++;
        if (questionIndex >= numbquestions) {
            quizCompleted = true;
        } else {
            resetCheckboxes();
        }
    }
}


void resetCheckboxes() {
  for (Checkbox cb : checkboxes) {
    cb.checked = false;
    cb.isFilled = false;
  }
}

void showThankYouMessage() {
  fill(textColor);
  textSize(24);
  textAlign(CENTER, CENTER);
  text("Thank you for completing the quiz!", width / 2, height / 2);
}

class Checkbox {
  float x, y;
  boolean checked = false;
  boolean isFilled = false;

  Checkbox(float x, float y) {
    this.x = x;
    this.y = y;
  }

  void display() {
    stroke(255);
    strokeWeight(2);
    if (isFilled) fill(checkboxColor);
    else noFill();
    rect(x - 10, y - 10, 20, 20, 5); // Center the checkbox by adjusting its position and add rounded corners
  }

  boolean isChecked(float mx, float my) {
    if (mx > x - 10 && mx < x + 10 && my > y - 10 && my < y + 10) {
      checked = !checked;
      return true;
    }
    return false;
  }
}

class Button {
    float x, y;
    float w, h;
    String label;
    color bgColor = color(70, 130, 180);
    color hoverColor = color(100, 160, 210);
    color textColor = color(255);
    boolean isHovered = false;

    Button(float x, float y, float w, float h, String label) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        this.label = label;
    }

    void display() {
        if (isHovered) {
            fill(hoverColor);
        } else {
            fill(bgColor);
        }
        noStroke();
        rect(x, y, w, h, 10); // Adicionar bordas arredondadas com raio de 10
        
        fill(textColor);
        textAlign(CENTER, CENTER);
        text(label, x + w / 2, y + h / 2);
    }

    boolean isClicked(float mx, float my) {
        return (mx > x && mx < x + w && my > y && my < y + h);
    }

    void checkHover(float mx, float my) {
        isHovered = (mx > x && mx < x + w && my > y && my < y + h);
    }
}


void mouseMoved() {
    startButton.checkHover(mouseX, mouseY);
    confirmButton.checkHover(mouseX, mouseY);
}
