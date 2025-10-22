import tkinter as tk
from tkinter import ttk
from tkinter import messagebox
from tkinter import filedialog
import json

class Pilot:
    def __init__(self, name="N/A", age=0, experience=0):
        self.name = name
        self.age = age
        self.experience = experience

    def __str__(self):
        return f"{self.name} (age: {self.age}, experience: {self.experience} years)"

    def Show(self):
        return str(self)
    def __del__(self):
        print("\nLaboratory work is done by a 2nd year student Danyliuk Yeghor")

class Plane:
    def __init__(self, model="Unknown", max_speed=0, capacity=0, pilot=None):
        self.model = model
        self.max_speed = max_speed
        self.capacity = capacity
        self.pilot = pilot if pilot else Pilot()

    def __str__(self):
        return f"Model: {self.model}, Speed: {self.max_speed} km/h, Capacity: {self.capacity}, Pilot: {self.pilot.Show()}"
    
    def get_info_for_file(self):
        return {
            'type': 'Plane',
            'model': self.model,
            'max_speed': self.max_speed,
            'capacity': self.capacity,
            'pilot': {
                'name': self.pilot.name,
                'age': self.pilot.age,
                'experience': self.pilot.experience
            }
        }

    def Show(self):
        return str(self)
        
    def get_description(self):
        return f"This is a civil or transport aircraft, model {self.model}."
    def __del__(self):
        print("\nLaboratory work is done by a 2nd year student Danyliuk Yeghor")

class Fighter(Plane):
    def __init__(self, model="Unknown", max_speed=0, capacity=0, pilot=None, ammo_capacity=0, stealth_range=0):
        super().__init__(model, max_speed, capacity, pilot)
        self.ammo_capacity = ammo_capacity
        self.stealth_range = stealth_range

    def __str__(self):
        base_info = super().__str__()
        return f"{base_info}, Ammo: {self.ammo_capacity}, Stealth Range: {self.stealth_range} km"
    
    def get_info_for_file(self):
        data = super().get_info_for_file()
        data['type'] = 'Fighter'
        data['ammo_capacity'] = self.ammo_capacity
        data['stealth_range'] = self.stealth_range
        return data

    def Show(self):
        return str(self)
        
    def get_description(self):
        return f"This is a combat fighter, model {self.model} with {self.ammo_capacity} units of ammo."
    def __del__(self):
        print("\nLaboratory work is done by a 2nd year student Danyliuk Yeghor")

class Hangar:
    def __init__(self, location="Unknown", capacity=10):
        self.location = location
        self.capacity = capacity
        self.planes = []

    def add_plane(self, plane):
        if len(self.planes) < self.capacity:
            self.planes.append(plane)
            return True
        return False

    def Show(self):
        if not self.planes:
            return "Hangar is empty."
        info = f"Hangar at location: {self.location}\n" + "-"*30 + "\n"
        for i, plane in enumerate(self.planes):
            info += f"{i+1}. {plane.Show()}\n"
        return info

    def save_to_file(self, filename):
        data_to_save = [plane.get_info_for_file() for plane in self.planes]
        try:
            with open(filename, 'w', encoding='utf-8') as f:
                json.dump(data_to_save, f, indent=4, ensure_ascii=False)
            return True
        except Exception as e:
            print(f"Error saving file: {e}")
            return False

    def load_from_file(self, filename):
        try:
            with open(filename, 'r', encoding='utf-8') as f:
                data = json.load(f)
                self.planes = []
                for item in data:
                    pilot_data = item['pilot']
                    pilot = Pilot(pilot_data['name'], pilot_data['age'], pilot_data['experience'])
                    
                    if item['type'] == 'Plane':
                        plane = Plane(item['model'], item['max_speed'], item['capacity'], pilot)
                    elif item['type'] == 'Fighter':
                        plane = Fighter(item['model'], item['max_speed'], item['capacity'], pilot,
                                        item.get('ammo_capacity', 0), item.get('stealth_range', 0))
                    self.add_plane(plane)
            return True
        except Exception as e:
            print(f"Error loading file: {e}")
            return False

    def __del__(self):
        print("\nLaboratory work is done by a 2nd year student Danyliuk Yeghor")

if __name__ == "__main__":
    hangar = Hangar("Central Airport", 20)
    root = tk.Tk()
    root.title("Hangar Management")

    entries = {}

    def update_display():
        text_area.config(state="normal")
        text_area.delete(1.0, tk.END)
        text_area.insert(tk.END, hangar.Show())
        text_area.config(state="disabled")

    def toggle_fighter_fields():
        if plane_type.get() == "Fighter":
            entries["Ammo Capacity"].config(state="normal")
            entries["Stealth Range"].config(state="normal")
        else:
            entries["Ammo Capacity"].config(state="disabled")
            entries["Stealth Range"].config(state="disabled")

    def add_plane_handler():
        try:
            pilot = Pilot(
                name=entries["Pilot Name"].get(),
                age=int(entries["Pilot Age"].get()),
                experience=int(entries["Pilot Experience"].get())
            )
            
            plane_choice = plane_type.get()
            if plane_choice == "Plane":
                plane = Plane(
                    model=entries["Model"].get(),
                    max_speed=int(entries["Max Speed"].get()),
                    capacity=int(entries["Capacity"].get()),
                    pilot=pilot
                )
            elif plane_choice == "Fighter":
                plane = Fighter(
                    model=entries["Model"].get(),
                    max_speed=int(entries["Max Speed"].get()),
                    capacity=int(entries["Capacity"].get()),
                    pilot=pilot,
                    ammo_capacity=int(entries["Ammo Capacity"].get()),
                    stealth_range=int(entries["Stealth Range"].get())
                )
            
            if hangar.add_plane(plane):
                messagebox.showinfo("Success", "Plane was successfully added to the hangar!")
            else:
                messagebox.showwarning("Error", "Hangar is full!")
            
            update_display()

        except ValueError:
            messagebox.showerror("Input Error", "Please check that numeric fields are filled correctly.")
        except Exception as e:
            messagebox.showerror("Unknown Error", f"An error occurred: {e}")

    def save_file_handler():
        filename = filedialog.asksaveasfilename(
            defaultextension=".json",
            filetypes=[("JSON files", "*.json"), ("All files", "*.*")]
        )
        if filename:
            if hangar.save_to_file(filename):
                messagebox.showinfo("Success", f"Data successfully saved to file:\n{filename}")
            else:
                messagebox.showerror("Error", "Failed to save file.")

    def load_file_handler():
        filename = filedialog.askopenfilename(
            filetypes=[("JSON files", "*.json"), ("All files", "*.*")]
        )
        if filename:
            if hangar.load_from_file(filename):
                messagebox.showinfo("Success", f"Data successfully loaded from file:\n{filename}")
                update_display()
            else:
                messagebox.showerror("Error", "Failed to load data from file.")

    # --- UI LAYOUT ---
    
    add_frame = ttk.LabelFrame(root, text="Add New Plane")
    add_frame.pack(padx=10, pady=10, fill="x")

    plane_type = tk.StringVar(value="Plane")
    ttk.Radiobutton(add_frame, text="Standard Plane", variable=plane_type, value="Plane", command=toggle_fighter_fields).grid(row=0, column=0, padx=5, pady=5)
    ttk.Radiobutton(add_frame, text="Fighter Plane", variable=plane_type, value="Fighter", command=toggle_fighter_fields).grid(row=0, column=1, padx=5, pady=5)

    fields = {
        "Model": "Boeing 747", "Max Speed": "988", "Capacity": "416",
        "Pilot Name": "John Doe", "Pilot Age": "45", "Pilot Experience": "20",
        "Ammo Capacity": "1000", "Stealth Range": "50"
    }
    
    row_num = 1
    for label_text, default_value in fields.items():
        ttk.Label(add_frame, text=label_text + ":").grid(row=row_num, column=0, sticky="w", padx=5, pady=2)
        entry = ttk.Entry(add_frame)
        entry.grid(row=row_num, column=1, sticky="ew", padx=5, pady=2)
        entry.insert(0, default_value)
        entries[label_text] = entry
        row_num += 1
        
    add_frame.columnconfigure(1, weight=1)

    add_button = ttk.Button(add_frame, text="Add Plane", command=add_plane_handler)
    add_button.grid(row=row_num, column=0, columnspan=2, pady=10)

    display_frame = ttk.LabelFrame(root, text="Hangar Contents")
    display_frame.pack(padx=10, pady=10, fill="both", expand=True)

    text_area = tk.Text(display_frame, wrap="word", height=15)
    text_area.pack(padx=5, pady=5, fill="both", expand=True)
    
    file_frame = ttk.LabelFrame(root, text="File Operations")
    file_frame.pack(padx=10, pady=10, fill="x")

    save_button = ttk.Button(file_frame, text="Save to File...", command=save_file_handler)
    save_button.pack(side="left", padx=5, pady=5)

    load_button = ttk.Button(file_frame, text="Load from File...", command=load_file_handler)
    load_button.pack(side="left", padx=5, pady=5)
    
    toggle_fighter_fields()
    update_display()

    root.mainloop()