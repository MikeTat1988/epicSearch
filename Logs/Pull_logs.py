import os
import subprocess
from datetime import datetime

def pull_logs():
    try:
        # Define the destination folder and log file path on the PC
        destination_folder = r"C:\Users\micha\ePicSearch\Logs"
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        destination_file = os.path.join(destination_folder, f"logs_{timestamp}.txt")

        # Ensure the destination folder exists
        if not os.path.exists(destination_folder):
            os.makedirs(destination_folder)

        # ADB command to read the logs.txt content
        adb_command = ["adb", "exec-out", "run-as", "com.epicsearch.app", "cat", "files/logs.txt"]

        # Execute the ADB command and save the output to the destination file
        with open(destination_file, "w") as f:
            result = subprocess.run(adb_command, stdout=f, stderr=subprocess.PIPE, text=True)

        if result.returncode == 0:
            print(f"Log file successfully copied to: {destination_file}")
        else:
            print(f"Error: {result.stderr}")
    except Exception as e:
        print(f"An error occurred: {e}")

if __name__ == "__main__":
    pull_logs()
