# Automatic processing of belly sprites using Aseprite CLI. Requires ASEPRITE in the %PATH% variable
# Requires a Python Interpeter
# This process is automated by Rider/VS if you use "RiderTerraria" configuration

import os
import subprocess
from threading import Thread

# from threading import Thread

# Path to the Aseprite executable
ASEPRITE_EXE = "aseprite.exe"

# Base folder containing subfolders with textures
BASE_FOLDER = "PlayerHandling\\TumSprites"

# Function to process a single .aseprite file
def process_file(aseprite_file, output_file):
	command = [
		ASEPRITE_EXE,
		"-b", aseprite_file, 
		"--scale", "2", 
		"--sheet", output_file, 
		"--sheet-type", "vertical"
	]
	print(f"Processing {aseprite_file}...")
	try:
		subprocess.run(command, check=True)
		print(f"Exported to {output_file}")
	except subprocess.CalledProcessError as e:
		print(f"Failed to process {aseprite_file}: {e}")

# Function to process all files in the base folder using threads
def process_folders_in_parallel(base_folder):
	threads = []
	
	# Remove all bare png
	#   for root, _, files in os.walk(base_folder):
	#	   for file in files:
	#		   if file.endswith("bare.png") or file.endswith("Bare.png"):
	#			   bareFilePath = os.path.join(root, file)
	#			   print("deleted " + bareFilePath)
	#			   os.remove(bareFilePath)
		
	for root, _, files in os.walk(base_folder):
		for file in files:
			if file.endswith(".aseprite"):
				# Full path to the .aseprite file
				aseprite_file = os.path.join(root, file)
				
				# Output file path (change extension to .png)
				output_file = os.path.join(root, os.path.splitext(file)[0] + ".png")
				
				process_file(aseprite_file, output_file)
				# Create a thread for each file
				thread = Thread(target=process_file, args=(aseprite_file, output_file))
				threads.append(thread)
				thread.start()
	
	# Wait for all threads to complete
	for thread in threads:
		thread.join()
	print("All files processed.")

# Run the script
if __name__ == "__main__":
	process_folders_in_parallel(BASE_FOLDER)
