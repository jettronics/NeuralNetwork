import numpy as np
import torch
import matplotlib.pyplot as plt
import pandas as pd
from torch.utils.data import Dataset, DataLoader

class CSVDataset(Dataset):
    def __init__(self, csv_file, mean=None, std=None, train=True):
        data = pd.read_csv(csv_file)
        X = data.iloc[:, :-1].values.astype(np.float32)
        y = data.iloc[:, -1].values.astype(np.int64)

        self.X = torch.tensor(X)
        self.y = torch.tensor(y)

        # Wenn train=True: berechne Normalisierung
        if train:
            self.mean = self.X.mean(dim=0)
            self.std = self.X.std(dim=0)
        else:
            self.mean = mean
            self.std = std

        # Wende Normalisierung an
        self.X = (self.X - self.mean) / self.std

    def __len__(self):
        return len(self.X)

    def __getitem__(self, idx):
        return self.X[idx], self.y[idx]
    
train_csv = "weather_classification_training_data_labelled.csv"
test_csv = "weather_classification_testing_data_labelled.csv"

train_data = CSVDataset(train_csv, train=True)
test_data = CSVDataset(test_csv, mean=train_data.mean, std=train_data.std, train=False)

train_loader = DataLoader(train_data, batch_size=32, shuffle=True)
test_loader = DataLoader(test_data, batch_size=32)

input_size = train_data.X.shape[1]
num_classes = len(torch.unique(train_data.y))
    
model = torch.nn.Sequential(
    torch.nn.Linear(input_size, 12), #10
    torch.nn.LeakyReLU(),
    torch.nn.Linear(12, 8),
    torch.nn.LeakyReLU(),
    torch.nn.Linear(8, num_classes)#, #4
    #torch.nn.Softmax()
)

epochs = 50
learning_rate = 1e-2

losses = []
optimizer = torch.optim.Adam(model.parameters(), lr=learning_rate) # alternative stochastic based: SGD

for epoch in range(epochs):
    model.train()
    running_loss = 0.0
    for X_batch, y_batch in train_loader:
        optimizer.zero_grad()
        probs = model(X_batch)
        criterion = torch.nn.CrossEntropyLoss()
        loss = criterion(probs, y_batch)
        losses.append(loss.item())
        loss.backward()
        optimizer.step()
        running_loss += loss.item()

    if (epoch + 1) % 10 == 0:
        print(f"Epoch [{epoch+1}/{epochs}] Loss: {running_loss/len(train_loader):.4f}")
        
model.eval()
correct, total = 0, 0
with torch.no_grad():
    for X_batch, y_batch in test_loader:
        probs = model(X_batch)
        preds = torch.argmax(probs, dim=1)
        total += y_batch.size(0)
        correct += (preds == y_batch).sum().item()
        for i in preds:
            print('%d expected %d' % (preds[i], y_batch[i]))

acc = 100 * correct / total
print(f"\n✅ Test Accuracy: {acc:.2f}%")

plt.figure(figsize=(10,7))
plt.plot(losses)
plt.xlabel("Epochs",fontsize=22)
plt.ylabel("Loss",fontsize=22)

#plt.savefig('C:\GitHub\\NeuralNetwork\Images\Weather_Prediction_PyTorch.jpg', format='jpg', dpi=200)
plt.show()




