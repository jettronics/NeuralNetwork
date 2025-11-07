import numpy as np
import torch
import matplotlib.pyplot as plt

# Tutorial:
# https://machinelearningmastery.com/introduction-to-softmax-classifier-in-pytorch/

# load the dataset, split into input (X) and output (y) variables
dataset = np.loadtxt('weather_classification_data.csv', skiprows=1, delimiter=',')
X = dataset[:,0:10] # every row, starting from 0, 10 values
y = dataset[:,10] # every row, 10th value starting from 0

# PyTorch is working with 32Bit
X = torch.tensor(X, dtype=torch.float32)
y = torch.tensor(y, dtype=torch.float32).reshape(-1, 1)

model = torch.nn.Sequential(
    torch.nn.Linear(10, 12),
    torch.nn.LeakyReLU(),
    torch.nn.Linear(12, 8),
    torch.nn.LeakyReLU(),
    torch.nn.Linear(8, 4),
    torch.nn.Softmax()
)

epochs = 800
batch = 200 
learning_rate = 1e-2

losses = []
# loss_fn = torch.nn.MSELoss() # alternative: BCELoss (binary cross entropy)
criterion = torch.nn.CrossEntropyLoss()
optimizer = torch.optim.SGD(model.parameters(), lr=learning_rate) # alternative stochastic based: SGD

for epoch in range(epochs):
    Xbatch = X[epoch]
    y_pred = model(Xbatch)
    ybatch = y[epoch]
    loss = criterion(y_pred, ybatch)
    losses.append(loss.item())
    optimizer.zero_grad()
    loss.backward()
    optimizer.step()
    print(f'Finished epoch {epoch}, latest loss {loss}')
        
# compute accuracy (no_grad is optional)
# with torch.no_grad():
y_pred = model(X)
accuracy = (y_pred.round() == y).float().mean()
print(f"Accuracy {accuracy}")

plt.figure(figsize=(10,7))
plt.plot(losses)
plt.xlabel("Epochs",fontsize=22)
plt.ylabel("Loss",fontsize=22)

#plt.savefig('C:\GitHub\\NeuralNetwork\Images\Heart_Disease_Prediction_PyTorch.jpg', format='jpg', dpi=200)
plt.show()

predictions = (model(X) > 0.5).int()
for i in range(420,470):
    print('%s => %d (expected %d)' % (X[i].tolist(), predictions[i], y[i]))
