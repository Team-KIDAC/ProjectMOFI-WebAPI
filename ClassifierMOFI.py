import tensorflow as tf
from tensorflow import keras
import numpy as np
from PIL import Image
import matplotlib.pyplot as plt


# Create a class for our Image classifier
class ClassifierMOFI:
    # Constructor.
    def __init__(self):
        self.IMG_HEIGHT = 128
        self.IMG_WIDTH = 128

        # Create an empty model
        imageClassifierModel = keras.Sequential([
            keras.layers.Rescaling(1. / 255, input_shape=(128, 128, 3)),
            keras.layers.Conv2D(16, 3, padding='same', activation='relu'),
            keras.layers.MaxPooling2D(),
            keras.layers.Conv2D(32, 3, padding='same', activation='relu'),
            keras.layers.MaxPooling2D(),
            keras.layers.Conv2D(64, 3, padding='same', activation='relu'),
            keras.layers.MaxPooling2D(),
            keras.layers.Flatten(),
            keras.layers.Dense(128, activation='relu'),
            keras.layers.Dense(3)
        ])

        imageClassifierModel.compile(optimizer='adam',
                                     loss=tf.keras.losses.SparseCategoricalCrossentropy(from_logits=True),
                                     metrics=['accuracy'])

        # imageClassifierModel.load_weights('DataSet/Model/cp.ckpt')
        self.probability_model = tf.keras.Sequential([imageClassifierModel,
                                                      tf.keras.layers.Softmax()])

    # Predict method for the image classifier.
    def predict(self, filePath):
        # Open image from filepath.
        img = Image.open(filePath)
        # Convert to an numpy array.
        imgArray = np.asarray(img)
        # Check whether the image is of shape RGB or RGBA.
        format = imgArray.shape[2]
        if format == 4:
            # convert RGBA to RGB
            background = Image.new("RGB", img.size, (255, 255, 255))
            background.paste(img, mask=img.split()[3])
            # Resize image
            new_img = background.resize((self.IMG_HEIGHT, self.IMG_WIDTH))
        else:
            new_img = img.resize((self.IMG_HEIGHT, self.IMG_WIDTH))

        new_img = np.asarray(new_img)
        # Normalise the image.
        # new_img = new_img / 255.0
        # Expand the dimension.
        new_img = (np.expand_dims(new_img, 0))

        # Feed the preprocessed image into the model and get the output
        self.predictionArray = self.probability_model.predict(new_img)

        # Get the index of the maximum prediction.
        pred_label = np.argmax(self.predictionArray[0])

        # Get the probability.
        self.probabilty = str(round((self.predictionArray[0][pred_label] * 100), 2))
        return pred_label

    # Method to print the probability
    def printBarGraph(self):
        objects = ('St_001', 'St_002', 'St_003', 'St_004', 'St_005')
        y_pos = np.arange(len(objects))
        plt.bar(y_pos, self.predictionArray[0], align='center', alpha=0.5)
        plt.xticks(y_pos, objects)
        plt.ylabel('Prediction Accuracy')
        plt.xlabel('Status Label')
        plt.title('Prediction')
        plt.show()
