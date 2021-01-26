import cv2, os, time
import numpy as np
import cv2, os, time
from yolov4.tf import YOLOv4
from mylib import config, thread

yolo = YOLOv4()
# yolo = YOLOv4(tiny=True)

yolo.classes = os.path.sep.join([config.MODEL_PATH, "coco.names"])
yolo.input_size = (640, 480)

yolo.make_model()
yolo.load_weights("yolov4.weights", weights_type="yolo")
# yolo.load_weights("yolov4-tiny.weights", weights_type="yolo")

yolo.inference(media_path="kite.jpg")

yolo.inference(media_path="road.mp4", is_image=False)