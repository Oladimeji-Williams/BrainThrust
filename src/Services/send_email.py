import smtplib
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText

# SMTP Configuration for Outlook
SMTP_SERVER = "smtp.office365.com"
EMAIL_PORT = 587
EMAIL_USERNAME = "oladimejiwilliams@outlook.com"
EMAIL_PASSWORD = "dphzsqitgktcevit"  # Replace with your generated App Password

# Email Details
FROM_EMAIL = EMAIL_USERNAME
TO_EMAIL = "mathematicianf@gmail.com"  # Replace with recipient's email
SUBJECT = "Test Email from Python"
BODY = "Hello, this is a test email sent from a Python script using Outlook SMTP."

def send_email():
    try:
        # Create SMTP session
        server = smtplib.SMTP(SMTP_SERVER, EMAIL_PORT)
        server.starttls()  # Secure the connection
        server.login(EMAIL_USERNAME, EMAIL_PASSWORD)  # Login with credentials

        # Create email message
        msg = MIMEMultipart()
        msg["From"] = FROM_EMAIL
        msg["To"] = TO_EMAIL
        msg["Subject"] = SUBJECT
        msg.attach(MIMEText(BODY, "plain"))

        # Send the email
        server.sendmail(FROM_EMAIL, TO_EMAIL, msg.as_string())

        # Close the SMTP connection
        server.quit()
        print("✅ Email sent successfully!")

    except Exception as e:
        print(f"❌ Error: {e}")

# Run the function
send_email()
