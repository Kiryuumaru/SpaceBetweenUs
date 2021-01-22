from mylib import config, thread
from mylib.mailer import Mailer
from mylib.detection import detect_people
from imutils.video import VideoStream, FPS
from scipy.spatial import distance as dist
import numpy as np
import imutils, cv2, os, time

# GUI
from guizero import App, Text, TextBox, PushButton, Slider, Picture

def start_internal():
	# load the COCO class labels our YOLO model was trained on
	labelsPath = os.path.sep.join([config.MODEL_PATH, "coco.names"])
	LABELS = open(labelsPath).read().strip().split("\n")

	# derive the paths to the YOLO weights and model configuration
	#weightsPath = os.path.sep.join([config.MODEL_PATH, "yolov2-tiny.weights"])
	#configPath = os.path.sep.join([config.MODEL_PATH, "yolov2-tiny.cfg"])
	weightsPath = os.path.sep.join([config.MODEL_PATH, "yolov3.weights"])
	configPath = os.path.sep.join([config.MODEL_PATH, "yolov3.cfg"])
	#weightsPath = os.path.sep.join([config.MODEL_PATH, "yolov3-tiny.weights"])
	#configPath = os.path.sep.join([config.MODEL_PATH, "yolov3-tiny.cfg"])
	#weightsPath = os.path.sep.join([config.MODEL_PATH, "yolov4-csp.weights"])
	#configPath = os.path.sep.join([config.MODEL_PATH, "yolov4-csp.cfg"])
	#weightsPath = os.path.sep.join([config.MODEL_PATH, "yolov4x-mish.weights"])
	#configPath = os.path.sep.join([config.MODEL_PATH, "yolov4x-mish.cfg"])

	# load our YOLO object detector trained on COCO dataset (80 classes)
	net = cv2.dnn.readNetFromDarknet(configPath, weightsPath)

	# check if we are going to use GPU
	if config.USE_GPU:
		# set CUDA as the preferable backend and target
		print("")
		print("[INFO] Looking for GPU")
		net.setPreferableBackend(cv2.dnn.DNN_BACKEND_CUDA)
		net.setPreferableTarget(cv2.dnn.DNN_TARGET_CUDA)

	# determine only the *output* layer names that we need from YOLO
	ln = net.getLayerNames()
	ln = [ln[i[0] - 1] for i in net.getUnconnectedOutLayers()]

	# grab a reference to the video file
	print("[INFO] Starting the video..")
	if config.Thread:
		cap = thread.ThreadingClass(config.test_vid)
	else:
		vs = cv2.VideoCapture(config.test_vid)

	# start the FPS counter
	fps = FPS().start()

	# loop over the frames from the video stream
	while True:
		# read the next frame from the file
		if config.Thread:
			frame = cap.read()
		else:
			(grabbed, frame) = vs.read()
			# if the frame was not grabbed, then we have reached the end of the stream
			if not grabbed:
				break

		# resize the frame and then detect people (and only people) in it
		frame = imutils.resize(frame, width=700)
		results = detect_people(frame, net, ln,
			personIdx=LABELS.index("person"))

		# initialize the set of indexes that violate the max/min social distance limits
		violation = set()

		# ensure there are *at least* two people detections (required in
		# order to compute our pairwise distance maps)
		if len(results) >= 2:
			# extract all centroids from the results and compute the
			# Euclidean distances between all pairs of the centroids
			centroids = np.array([r[2] for r in results])
			D = dist.cdist(centroids, centroids, metric="euclidean")

			# loop over the upper triangular of the distance matrix
			for i in range(0, D.shape[0]):
				for j in range(i + 1, D.shape[1]):
					# check to see if the distance between any two
					# centroid pairs is less than the configured number of pixels
					if D[i, j] < config.MIN_DISTANCE:
						# update our violation set with the indexes of the centroid pairs
						violation.add(i)
						violation.add(j)

		# loop over the results
		for (i, (prob, bbox, centroid)) in enumerate(results):
			# extract the bounding box and centroid coordinates, then
			# initialize the color of the annotation
			(startX, startY, endX, endY) = bbox
			(cX, cY) = centroid
			color = (0, 255, 0)

			# if the index pair exists within the violation/abnormal sets, then update the color
			if i in violation:
				color = (0, 0, 255)

			# draw (1) a bounding box around the person and (2) the
			# centroid coordinates of the person,
			cv2.rectangle(frame, (startX, startY), (endX, endY), color, 2)
			cv2.circle(frame, (cX, cY), 5, color, 2)

		# draw some of the parameters
		Threshold = "Threshold limit: {}".format(config.Threshold)
		cv2.putText(frame, Threshold, (470, frame.shape[0] - 50),
			cv2.FONT_HERSHEY_SIMPLEX, 0.60, (255, 0, 0), 2)

		# draw the total number of social distancing violations on the output frame
		text = "Total violations: {}".format(len(violation))
		cv2.putText(frame, text, (470, frame.shape[0] - 25),
			cv2.FONT_HERSHEY_SIMPLEX, 0.70, (0, 0, 255), 2)

	#------------------------------Alert function----------------------------------#
		if len(violation) >= config.Threshold:
			cv2.putText(frame, "-ALERT: Violations over limit-", (10, frame.shape[0] - 80),
				cv2.FONT_HERSHEY_COMPLEX, 0.60, (0, 0, 255), 2)
			if config.ALERT:
				print("")
				print('[INFO] Sending mail...')
				Mailer().send(config.MAIL)
				print('[INFO] Mail sent')
			#config.ALERT = False
	#------------------------------------------------------------------------------#
		# show the output frame
		cv2.imshow("Real-Time Monitoring/Analysis Window", frame)
		key = cv2.waitKey(1) & 0xFF

		# if the `q` key was pressed, break from the loop
		if key == ord("q"):
			break
		# update the FPS counter
		fps.update()

	# stop the timer and display FPS information
	fps.stop()
	print("===========================")
	print("[INFO] Elasped time: {:.2f}".format(fps.elapsed()))
	print("[INFO] Approx. FPS: {:.2f}".format(fps.fps()))

	# close any open windows
	cv2.destroyAllWindows()

def say_my_name():
    welcome_message.value = my_name.value

def change_text_size(slider_value):
    welcome_message.size = slider_value

app = App(title="Hello world")

app.set_full_screen()
welcome_message = Text(app, text="Welcome to my app", size=40, font="Times New Roman", color="lightblue")
my_name = TextBox(app)
update_text = PushButton(app, command=say_my_name, text="Display my name")
openCV = PushButton(app, command=start_internal, text="OpenCV")
text_size = Slider(app, command=change_text_size, start=10, end=80)

app.display()
